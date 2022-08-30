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
        /// Contem todos os dados genericos, gerenciados pelo GMB.
        /// <para>
        /// - Necessario cast para saber que tipo de dado esta sendo retornado em sua pesquisa. Nao indicado pesquisas nesta lista caso nao seja realmente necesario, pois
        /// pode conter muitos dados para iteracao. Para pesquisas eficientes utilize <see cref="GetDatas{T}"/>, do qual pesquisara dados baseado no catalogo!
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
            foreach (Data data in _allDatas)
            {
                EvalueDataType(data);
            }
        }
        private void EvalueDataType(Data data)
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
        /// Informa se os dados estao prontos para serem utilizados por terceiros apartir da variavel <see cref="IsReady"/>
        /// </summary>
        /// <returns></returns>
        public abstract bool GetIsReady();
    }
}
