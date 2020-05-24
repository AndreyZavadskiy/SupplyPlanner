namespace SP.Service.Models
{
    /// <summary>
    /// Элемент списка пользователей
    /// </summary>
    public class UserListItem
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Описание роли
        /// </summary>
        public string RoleDescription { get; set; }
        /// <summary>
        /// Описание территорий по своей зоне ответственности
        /// </summary>
        public string TerritoryDescription { get; set; }
        /// <summary>
        /// Является активным пользователем
        /// "1" - активный, "0" - заблокирован
        /// </summary>
        public string Active { get; set; }
    }
}
