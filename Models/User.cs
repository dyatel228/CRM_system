using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Web.Models
{
    public class User
    {
        [Display(Name = "ID")]
        public long Id { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Фамилия не должна превышать 100 символов")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Отчество")]
        [StringLength(100, ErrorMessage = "Отчество не должно превышать 100 символов")]
        public string? Patronymic { get; set; } 

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Введите корректный email адрес")]
        [StringLength(255, ErrorMessage = "Email не должен превышать 255 символов")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Введите корректный номер телефона")]
        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Роль")]
        [StringLength(50, ErrorMessage = "Роль не должна превышать 50 символов")]
        public string Role { get; set; } = "менеджер";

        [Display(Name = "Дата создания")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string GetFullName()
        {
            string fullName = $"{LastName} {FirstName}";
            if (!string.IsNullOrWhiteSpace(Patronymic))
            {
                fullName += $" {Patronymic}";
            }
            return fullName.Trim();
        }
    }
}