using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Web.Models
{
    public class Client
    {
        [Display(Name = "ID")]
        public long Id { get; set; }

        [Display(Name = "Название компании")]
        [Required(ErrorMessage = "Введите название компании")]
        [StringLength(255, ErrorMessage = "Название слишком длинное")]
        public string CompanyName { get; set; }

        [Display(Name = "ИНН")]
        [Required(ErrorMessage = "Введите ИНН")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать 10 или 12 цифр")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ИНН должен содержать только цифры")]
        public string INN { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Введите телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [StringLength(20, ErrorMessage = "Телефон слишком длинный")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите правильный email адрес")]
        [StringLength(255, ErrorMessage = "Email слишком длинный")]
        public string Email { get; set; }

        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Введите адрес")]
        [StringLength(500, ErrorMessage = "Адрес слишком длинный")]
        public string Address { get; set; }

        [Display(Name = "Статус")]
        [Required(ErrorMessage = "Выберите статус")]
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