using System.Collections;
using System.Collections.Generic;
using GMB;
using System.Linq;
using UnityEngine;
using System;

namespace GMB.Database
{
    /// <summary>
    /// Carrega todos os dados do GMBEditor e mantem os dados em catalogo, baseado em tipos, para tornar pesquisas futuras mais eficients.
    /// <para>
    /// - Utilize <see cref="GetDatas{T}"/>, para pesquisar dados catalogados por tipos.
    /// </para>
    /// <para>
    /// - Implemente esta abstracao para criar meios eficientes de dados em cache apartir dos atuais dados ja catalogados.
    /// </para>
    /// 
    /// </summary>
    public abstract class GMBDatabaseBehaviour : MonoBehaviour
    {
        private static GMBDatabaseBehaviour _instance = null;
        public static T GetInstance<T>() where T : GMBDatabaseBehaviour
        {
            return (T)_instance;
        }
        /// <summary>
        /// Contem todos os dados genericos, gerenciados pelo GMB. Esta lista possui uma ordenacao fixa e confiavel, persistente em instancias de inicializacao.
        /// <para>
        /// - Necessario cast para saber que tipo de dado esta sendo retornado em sua pesquisa.
        /// Nao indicado pesquisas nesta lista caso nao seja realmente necesario, pois
        /// pode conter muitos dados para iteracao. Para pesquisas eficientes utilize <see cref="GetDatas{T}"/>, e os outros metodos de pesquisas eficientes
        /// </para>
        ///
        /// <para>
        /// - Utilize <see cref="GetDataIndex(Data)"/> para saber o indice de alocacao do dado na lista geral. Voce podera utilizar com seguranca este indice para criacao de buffers de pesquisa
        /// rapida
        /// </para>
        ///
        /// <para>
        /// - Utilize <see cref="GetDataFromID(string)"/> ou <see cref="GetDataFromAID(int)"/> para persistencia de dados confiavel
        /// </para>
        /// </summary>
        protected List<Data> _allDatas = new List<Data>();

        /// <summary>
        /// Catalogo de dados cadastrados por tipo. Tais como <see cref="Data_Item"/>, <see cref="Data_ItemAttribute"/>, etc.
        /// <para>
        /// Esta catálogo contem todos os dados gerenciados pelo GMB, catalogados por tipo de dado.
        /// Utilize <see cref="GetDatas{T}"/> para obter o dado do tipo especifico de maneira eficente.
        /// </para>
        /// </summary>
        protected Dictionary<Type, List<Data>> _typeDatas_catalogue = new Dictionary<Type, List<Data>>();

        /// <summary>
        /// Contem o idice do dado na lista geral de dados cadastrados <see cref="_allDatas"/>. 
        /// </summary>
        protected Dictionary<Data, int> _dataIndex_catalogue = new Dictionary<Data, int>();
        /// <summary>
        /// Contem dados catalogados por <see cref="Data.GetID"/>.
        /// - Utilize es
        /// </summary>
        protected Dictionary<string, Data> _dataID_catalogue = new Dictionary<string, Data>();
        /// <summary>
        /// Contem dados catalogados por <see cref="Data.GetAID"/>
        /// </summary>
        protected Dictionary<int, Data> _dataAID_catalogue = new Dictionary<int, Data>();

        /// <summary>
        /// Utilize para informar a terceiros que seus dados estao prontos para serem gerenciados.
        /// </summary>
        public bool IsReady => GetIsReady();


        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDatas();
        }

        private void LoadDatas()
        {
            _allDatas = Resources.LoadAll<Data>(StringsProvider._RELATIVE_PATH_DATAS_).OrderBy(r => r.GetTID()).ToList();

            CreateDataCatalogue();
        }
        private void CreateDataCatalogue()
        {
            int index = 0;
            foreach (Data data in _allDatas)
            {
                EvalueDataTypeCatalogue(data);
                EvalueDataIDCatalogue(data);
                EvalueDataIndexCatalogue(index, data);
                index++;
            }
        }
        private void EvalueDataTypeCatalogue(Data data)
        {
            Type type = data.GetType();

            if (_typeDatas_catalogue.ContainsKey(type))
            {
                _typeDatas_catalogue[type].Add(data);
            }
            else
            {
                _typeDatas_catalogue.Add(type, new List<Data>());
                _typeDatas_catalogue[type].Add(data);
            }
        }
        private void EvalueDataIndexCatalogue(int index, Data data)
        {
            _dataIndex_catalogue.Add(data, index);

        }
        private void EvalueDataIDCatalogue(Data data)
        {
            _dataID_catalogue.Add(data.GetID(), data);  //ID - GUID, unico do arquivo
            _dataAID_catalogue.Add(data.GetAID(), data); //ID Numero AutoIncrementado, Unico
        }


        /// <summary>
        /// Procura o tipo de dado em cache e retorna uma lista contendo todos os dados deste tipo.
        /// <para>
        /// - Tenha certeza de que <see cref="GMBDatabase"/> componente tenha sido previamente carregado em alguma cena!
        /// </para>
        /// <para>
        /// - Utilize, <see cref="LoadDatas(Action)"/></para> para garantir que todos os tipos de dados tenham sido previamente carregados.
        /// <para>
        /// - Utilize <see cref="IsReady"/></para> para ter certeza que todos os dados foram crregados e finalizados.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetDatas<T>() where T : Data
        {
            List<T> datas = new List<T>();

            if (_typeDatas_catalogue.TryGetValue(typeof(T), out List<Data> result))
            {
                datas = result.Select(r => r as T).ToList();
            }

            return datas;
        }

        /// <summary>
        /// Recebe o indice do dado na lista geral de dados <see cref="_allDatas"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetDataIndex(Data data)
        {
            try
            {
                return _dataIndex_catalogue[data];
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return -1;
        }



        /// <summary>
        /// Atraves do indice de acesso, avalia e retorna o dado correspondente.
        /// Este e um  metodo rapido e eficaz para acessar os dados cadastrados. Voce pode utilizar <see cref="GetDataIndex(Data)"/>, para saber qual o indice de terminado dado.
        /// <para>
        /// - Nao recomendado armazenar o indice do dado em banco de dados, pois havendo remocao de arquivos no GMB, o indice em futuras inicializacoes sera diferente de versoes anteriores.
        /// - Para armazenamento utilize <see cref="Data.GetAID"/> ou <see cref="Data.GetID"/>
        /// </para>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Data GetDataFromIndex(int index)
        {
            try
            {
                return _allDatas[index];
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return null;
        }
        /// <summary>
        /// Atraves do Id GUID: <see cref="Data.GetID"/>, avalia e retorna o dado correspondente.
        /// Este e um  metodo rapido e eficaz para acessar os dados cadastrados. Para persistencia de dados utilize <see cref="Data.GetAID"/>, e <see cref="GetDataFromAID(int)"/>
        /// </summary>
        /// <param name="dataID">GUID unico do arquivo, <see cref="Data.GetID"/></param>
        /// <returns>Null se nao houver nenhum cadastrado pelo GMB</returns>
        public Data GetDataFromID(string dataID)
        {

            try
            {
                return _dataID_catalogue[dataID];
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return null;
        }

        /// <summary>
        /// Atraves do ID auto incrementado do arquivo, <see cref="Data.GetAID"/>, avalia e retorna o dado correspondente.
        /// Este e um metodo rapido e eficaz para recoperar um dado. Voce pode utilizar utilizar este ID em persistencia de dados. 
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        public Data GetDataFromAID(int dataID)
        {

            try
            {
                return _dataAID_catalogue[dataID];
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return null;
        }


        /// <summary>
        /// Informa se os dados estao prontos para serem utilizados por terceiros apartir da variavel <see cref="IsReady"/>
        /// </summary>
        /// <returns></returns>
        public abstract bool GetIsReady();
    }
}
