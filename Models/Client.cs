using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Web.Models
{
    public class Client
    {
        [Display(Name = "ID")]
        public long Id { get; set; }

        [Display(Name = "Название компании")]
        [Required(ErrorMessage = "Поле 'Название компании' обязательно для заполнения")]
        [StringLength(255, ErrorMessage = "Название компании не должно превышать 255 символов")]
        public string CompanyName { get; set; }

        [Display(Name = "ИНН")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать 10 или 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ИНН должен содержать только цифры")]
        public string INN { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
        [StringLength(255, ErrorMessage = "Email не должен превышать 255 символов")]
        public string Email { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Статус")]
        [Required(ErrorMessage = "Поле 'Статус' обязательно для заполнения")]
        public string Status { get; set; }

        [Display(Name = "Дата создания")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Ответственный менеджер")]
        public long? ResponsibleUserId { get; set; }

        public Client()
        {
            Status = "Лид";
            CreatedDate = DateTime.Now;
        }
    }
}