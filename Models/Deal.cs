using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Web.Models
{
    public class Deal
    {
        [Display(Name = "ID")]
        public long Id { get; set; }

        [Display(Name = "Название сделки")]
        [Required(ErrorMessage = "Поле 'Название сделки' обязательно для заполнения")]
        [StringLength(255, ErrorMessage = "Название сделки не должно превышать 255 символов")]
        public string DealName { get; set; }

        [Display(Name = "Клиент")]
        [Required(ErrorMessage = "Необходимо выбрать клиента")]
        public long ClientId { get; set; }

        [Display(Name = "Сумма")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Стадия")]
        public string Stage { get; set; }

        [Display(Name = "Ответственный менеджер")]
        public long? ResponsibleUserId { get; set; }

        [Display(Name = "Дата создания")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Срок исполнения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DeadlineDate { get; set; }

        public Deal()
        {
            Stage = "Переговоры";
            CreatedDate = DateTime.Now;
        }
    }
}