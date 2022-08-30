using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMB;
using System;
using System.Linq;

namespace GMB.Database
{

    /// <summary>
    /// Carrega e mantem em cache todos os dados gerenciados pelo GMB (Game manager Backend Editor).
    /// <para>
    /// - Este componente persistirá em carregamentos de cenas posteriores e tornará possiel a utilizacao de metodos de pesquisa para gerenciamento dos dados cadastrados no GMB.
    /// </para>
    /// <para>
    /// - Tenha certeza de ter este componente carregado logo que possivel, para permitir que as funcoes estaticas de pesquisa, trabalhem corretamente.
    /// </para>
    /// <para>
    /// - Utilize os metodos estaticos de <see cref="GMBDatabaseExtensions"/> para fazer pesquisas de dados diretamente na fonte da pesquisa!
    /// </para>
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public partial class GMBDatabase : GMBDatabaseBehaviour
    {
        public static GMBDatabase Instance { get { return GetInstance<GMBDatabase>(); } }

        /// <summary>
        /// Catalogo contendo todos os ingredientes (<see cref="Data_ItemIngredient"/>), de um item (<see cref="Data_Item"/>), origem.
        /// </summary>
        private Dictionary<Data_Item, List<Data_ItemIngredient>> _item_recipes_catalogue = new Dictionary<Data_Item, List<Data_ItemIngredient>>();

        /// <summary>
        /// Catalogo contendo todos os craftables (<see cref="Data_ItemCrafter"/>), de um item (<see cref="Data_Item"/>), de origem.
        /// </summary>
        private Dictionary<Data_Item, List<Data_ItemCrafter>> _item_crafts_catalogue = new Dictionary<Data_Item, List<Data_ItemCrafter>>();

        /// <summary>
        /// Catalogo de itens (<see cref="Data_Item"/>), em uma cateogria (<see cref="Data_ItemCategory"/>), de item especifica.
        /// </summary>
        private Dictionary<Data_ItemCategory, List<Data_Item>> _itemsByCategory_catalogue = new Dictionary<Data_ItemCategory, List<Data_Item>>();

        /// <summary>
        /// Verifique se o item craft, faz parte da lista de itens de craftables(<see cref="Data_ItemCrafter"/>), do item origem.
        /// </summary>
        /// <param name="item"item origem. Item que voce deseja saber se possui o item craft em sua lista de craftables></param>
        /// <param name="craft">verifica se este item faz parte da lista de craftables (<see cref="Data_ItemCrafter"/>), do item origem</param>
        /// <returns></returns>
        public bool HasCrafter(Data_Item item, Data_Item craft)
        {
            return GetCrafters(item).Exists(r => r.GetItem() == craft);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        public bool HasIngredient(Data_Item item, Data_Item ingredient)
        {
            return GetRecipe(item).Exists(r => r.GetItem() == ingredient);
        }
        /// <summary>
        /// Verifica se o item origem possui todos os itens de ingredientes em sua lista de receita. <see cref="Data_ItemIngredient"/>.
        /// <para>
        /// O Item origem pode conter mais itens do que a lista infromada de ingredientes que voce esta pesquisando, isto nao vai afetar o resultado desta pesquisa.
        /// Verdadeiro sera retornado se todos os itens que vc esta pequisando, estiverem na lista de receita o item origem.
        /// </para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ingredients">Lista de itens que voce deseja saber se fazem parte da da receita o item origem.</param>
        /// <returns>Retorna verdadeiro caso todos os itens da lista estejam presentaes na lista de receita do item origem.</returns>
        public bool HasAllIngredients(Data_Item item, List<Data_Item> ingredients)
        {
            bool result = true;

            foreach (var ingredient in ingredients)
            {
                if (HasIngredient(item, ingredient))
                    continue;

                result = false;
                break;
            }

            return result;
        }
        /// <summary>
        /// Verifica se o item origem possui posui em sua lista de receita, exatamente os mesmos itens de ingredientes que vc esta pesquisando.
        /// <para>
        /// O Item origem deve conter somente os itens de ingredientes que voce esta pesquisando, nada mais e nada menos!
        /// Verdadeiro sera retornado se o item origem tiver exatamente os mesmos items de ingredientes que vc esta pequisando.
        /// </para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ingredients">Lista de itens que voce deseja saber se fazem parte da da receita o item origem.</param>
        /// <returns>Retorna verdadeiro caso todos os itens da lista estejam presentaes na lista de receita do item origem.</returns>
        public bool HasAllIngredientsCounted(Data_Item item, List<Data_Item> ingredients)
        {
            bool result = true;

            if (GetRecipeCount(item) != ingredients.Count)
            {
                result = false;
                return result; ;
            }

            foreach (var ingredient in ingredients)
            {
                if (HasIngredient(item, ingredient))
                    continue;

                result = false;
                break;
            }

            return result;
        }
        /// <summary>
        /// Recebe uma lista contendo todos os itens de ingredientes utilizados na receita do item origem
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a receita</param>
        /// <param name="recipe"> Lista contendo Todos os ingredientes para fabricar o item</param>
        /// <returns>Falso caso o item nao tenha nenhum ingrediente como receita</returns>
        public bool TryGetRecipe(Data_Item item, out List<Data_ItemIngredient> recipe)
        {
            return _item_recipes_catalogue.TryGetValue(item, out recipe);
        }
        /// <summary>
        /// Recebe uma lista contendo todos os itens de ingredientes utilizados na receita do item origem
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a receita</param>
        /// <returns></returns>
        public List<Data_ItemIngredient> GetRecipe(Data_Item item)
        {
            _item_recipes_catalogue.TryGetValue(item, out var recipe);
            return recipe;
        }
        /// <summary>
        /// Informa a quantidade de itens necessarias para a receita do item origem
        /// </summary>
        /// <param name="item">Itm origem para o qual vc deseja saber quantos ingredientes e necessario para a receita</param>
        /// <returns></returns>
        public int GetRecipeCount(Data_Item item)
        {
            return _item_recipes_catalogue[item].Count;
        }
        /// <summary>
        /// Recebe uma lista de itens do qual o itemItem origem, que voce deseja verificar a receita origem pode ser construido.
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a lista de equipamentos avalidas para a construcao</param>
        /// <param name="crafters">Lista de todos os itens de construcao do qual o item origem pode ser fabricado. Vazio caso nao haja nenhum</param>
        /// <returns>Falso se o item nao tiver nenhum craftable em sua lista</returns>       
        public bool TryGetCrafters(Data_Item item, out List<Data_ItemCrafter> crafters)
        {
            return _item_crafts_catalogue.TryGetValue(item, out crafters);
        }
        /// <summary>
        /// Recebe uma lista de itens do qual o itemItem origem, que voce deseja verificar a receita origem pode ser construido.
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a lista de equipamentos avalidas para a construcao</param>
        /// <returns></returns>       
        public List<Data_ItemCrafter> GetCrafters(Data_Item item)
        {
            _item_crafts_catalogue.TryGetValue(item, out var crafters);
            return crafters;
        }
        /// <summary>
        /// Recebe uma lista contendo todos os itens que pertencem a uma categoria informada
        /// </summary>
        /// <param name="category"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool TryGetItemsByCategory(Data_ItemCategory category, out List<Data_Item> items)
        {

            items = GetItemsByCategory(category);

            if (items == null)
                items = new List<Data_Item>();

            return items.Count > 0;

        }
        /// <summary>
        /// Recebe uma lista contendo todos os itens que pertencem a uma categoria informada
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Data_Item> GetItemsByCategory(Data_ItemCategory category)
        {
            if (category == null)
            {
                return GetDatas<Data_Item>().Where(r => r.GetCategory() == null).ToList();
            }

            return _itemsByCategory_catalogue[category];
        }

        private bool _itemRecipesReady = false;
        private bool _itemCraftersReady = false;
        private bool _itemsByCategoryReady = false;
        private IEnumerator corotine = null;

        public void LoadDatasAsync(Action finishedCallback)
        {
            if (corotine != null) { return; }
            corotine = EvalueCatalogueAsync(finishedCallback);
            StartCoroutine(corotine);
        }

        IEnumerator EvalueCatalogueAsync(Action finishCallback)
        {


            StartCoroutine(EvalueItemRecipeAsync());
            StartCoroutine(EvalueItemCraftsAsync());
            StartCoroutine(EvalueItemsByCategoryAsync());


            while (!IsReady)
            {
                yield return null;
            }

            finishCallback?.Invoke();
            corotine = null;
        }
        IEnumerator EvalueItemRecipeAsync()
        {

            List<Data_Item> items = GetDatas<Data_Item>();

            //catalogando os itens
            foreach (Data_Item item in items)
            {
                if (_item_recipes_catalogue.ContainsKey(item) == false)
                {
                    _item_recipes_catalogue.Add(item, new List<Data_ItemIngredient>());
                }

            }

            //catalogando os ingredients dos itens
            List<Data_ItemIngredient> ingredients = GetDatas<Data_ItemIngredient>();
            foreach (var ingredient in ingredients)
            {
                var item = ingredient.GetOwner();
                _item_recipes_catalogue[item].Add(ingredient);
            }

            _itemRecipesReady = true;

            yield return null;
        }
        IEnumerator EvalueItemCraftsAsync()
        {

            List<Data_Item> items = GetDatas<Data_Item>();

            //catalogando os itens
            foreach (Data_Item item in items)
            {
                if (_item_crafts_catalogue.ContainsKey(item) == false)
                {
                    _item_crafts_catalogue.Add(item, new List<Data_ItemCrafter>());
                }

            }

            //catalogando os crafs dos itens
            List<Data_ItemCrafter> crafters = GetDatas<Data_ItemCrafter>();
            foreach (var crafter in crafters)
            {
                var item = crafter.GetOwner();
                _item_crafts_catalogue[item].Add(crafter);
            }

            _itemCraftersReady = true;
            yield return null;
        }
        IEnumerator EvalueItemsByCategoryAsync()
        {

            List<Data_ItemCategory> categories = GetDatas<Data_ItemCategory>();

            //catalogando as categorias
            foreach (Data_ItemCategory category in categories)
            {
                if (_itemsByCategory_catalogue.ContainsKey(category) == false)
                {
                    _itemsByCategory_catalogue.Add(category, new List<Data_Item>());
                }

            }

            //catalogando os itens em suas devidas categorias
            List<Data_Item> items = GetDatas<Data_Item>();
            foreach (var item in items)
            {
                if (item.GetCategory() != null)
                {
                    _itemsByCategory_catalogue[item.GetCategory()].Add(item);
                }

            }

            _itemsByCategoryReady = true;
            yield return null;
        }

        public override bool GetIsReady()
        {
            return _itemRecipesReady && _itemCraftersReady && _itemsByCategoryReady;
        }
    }
}