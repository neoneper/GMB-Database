using UnityEngine;
using System.Collections;
using GMB;
using System.Collections.Generic;

namespace GMB.Database
{
    public static class GMBDatabaseExtensions
    {
     
        /// <summary>
        /// Verifique se o ingrediente faz parte da lista receita (<see cref="Data_ItemIngredient"/>), do item origem
        /// </summary>
        /// <param name="item_origin">item origem, item que voce deseja saber se contem o item ingrediente em sua lista de receita</param>
        /// <param name="ingredient">verifica se este item faz parte dos ingredientes da receita do item</param>
        /// <returns>Retorna verdadeiro se o ingredient fazer parte da receita do item</returns>
        public static bool HasItemIngredient(this Data_Item item_origin, Data_Item ingredient)
        {
            return GMBDatabase.Instance.HasIngredient(item_origin, ingredient);
        }

        /// <summary>
        /// Verifique se o item craft, faz parte da lista de itens de craftables(<see cref="Data_ItemCrafter"/>), do item origem.
        /// </summary>
        /// <param name="item_origim"item origem. Item que voce deseja saber se possui o item craft em sua lista de craftables></param>
        /// <param name="craft">verifica se este item faz parte da lista de craftables (<see cref="Data_ItemCrafter"/>), do item origem</param>
        /// <returns></returns>
        public static bool HasItemCrafter(this Data_Item item_origim, Data_Item craft)
        {
            return GMBDatabase.Instance.HasCrafter(item_origim, craft);
        }

        /// <summary>
        /// Verfique se este item possui uma categoria (<see cref="Data_ItemCategory"/>), valida.
        /// </summary>
        /// <param name="item">Item que voce deseja saber se possui uma <see cref="Data_ItemCategory"/> valida </param>
        /// <returns></returns>
        public static bool HasItemCategory(this Data_Item item)
        {
            return item.GetCategory() != null;
        }

        /// <summary>
        /// Verifica se o item origem possui todos os itens de ingredientes em sua lista de receita. <see cref="Data_ItemIngredient"/>.
        /// <para>
        /// O Item origem pode conter mais itens do que a lista infromada de ingredientes que voce esta pesquisando, isto nao vai afetar o resultado desta pesquisa.
        /// Verdadeiro sera retornado se todos os itens que vc esta pequisando, estiverem na lista de receita o item origem.
        /// </para>
        /// </summary>
        /// <param name="item_origim"></param>
        /// <param name="ingredients">Lista de itens que voce deseja saber se fazem parte da da receita o item origem.</param>
        /// <returns>Retorna verdadeiro caso todos os itens da lista estejam presentaes na lista de receita do item origem.</returns>
        public static bool HasItemAllIngredients(this Data_Item item_origim, List<Data_Item> ingredients)
        {
            return GMBDatabase.Instance.HasAllIngredients(item_origim, ingredients);
        }
        /// <summary>
        /// Verifica se o item origem possui posui em sua lista de receita, exatamente os mesmos itens de ingredientes que vc esta pesquisando.
        /// <para>
        /// O Item origem deve conter somente os itens de ingredientes que voce esta pesquisando, nada mais e nada menos!
        /// Verdadeiro sera retornado se o item origem tiver exatamente os mesmos items de ingredientes que vc esta pequisando.
        /// </para>
        /// </summary>
        /// <param name="item_origim"></param>
        /// <param name="ingredients">Lista de itens que voce deseja saber se fazem parte da da receita o item origem.</param>
        /// <returns>Retorna verdadeiro caso todos os itens da lista estejam presentaes na lista de receita do item origem.</returns>
        public static bool HasItemAllIngredientsCounted(this Data_Item item, List<Data_Item> ingredients)
        {
            return GMBDatabase.Instance.HasAllIngredientsCounted(item, ingredients);
        }
        /// <summary>
        /// Recebe uma lista contendo todos os itens de ingredientes utilizados na receita do item origem
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a receita</param>
        /// <returns></returns>
        public static List<Data_ItemIngredient> GetItemRecipe(this Data_Item item)
        {
            return GMBDatabase.Instance.GetRecipe(item);
        }

        /// <summary>
        /// Recebe uma lista de itens do qual o itemItem origem, que voce deseja verificar a receita origem pode ser construido.
        /// </summary>
        /// <param name="item">Item origem, que voce deseja verificar a lista de equipamentos avalidas para a construcao</param>
        /// <returns></returns>       
        public static List<Data_ItemCrafter> GetItemCrafters(this Data_Item item)
        {
            return GMBDatabase.Instance.GetCrafters(item);
        }

        /// <summary>
        /// Informa a quantidade de itens necessarias para a receita do item origem
        /// </summary>
        /// <param name="item">Itm origem para o qual vc deseja saber quantos ingredientes e necessario para a receita</param>
        /// <returns></returns>
        public static int GetItemRecipeCount(this Data_Item item)
        {
            return GMBDatabase.Instance.GetRecipeCount(item);
        }

        /// <summary>
        /// Recebe uma lista contendo todos os itens que pertencem a uma categoria informada
        /// </summary>
        /// <param name="itemCategory"></param>
        /// <returns></returns>
        public static List<Data_Item> GetItemsAtCategory(this Data_ItemCategory itemCategory)
        {
            return GMBDatabase.Instance.GetItemsByCategory(itemCategory);
        }
    }

}