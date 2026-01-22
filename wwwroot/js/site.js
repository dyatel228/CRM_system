// Плавная прокрутка для якорей
document.addEventListener('DOMContentLoaded', function () {
    // Анимация появления элементов
    const animateOnScroll = function () {
        const elements = document.querySelectorAll('.fade-in');

        for (let i = 0; i < elements.length; i++) {
            const element = elements[i];
            const elementPosition = element.getBoundingClientRect().top;
            const screenPosition = window.innerHeight / 1.3;

            if (elementPosition < screenPosition) {
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            }
        }
    };

    // Инициализация анимации
    window.addEventListener('scroll', animateOnScroll);
    animateOnScroll();

    // Подтверждение удаления
    const deleteForms = document.querySelectorAll('form.delete-form');

    for (let i = 0; i < deleteForms.length; i++) {
        deleteForms[i].addEventListener('submit', function (e) {
            if (!confirm('Вы уверены, что хотите удалить эту запись?')) {
                e.preventDefault();
            }
        });
    }

    // Подсветка активной навигации
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    for (let i = 0; i < navLinks.length; i++) {
        const link = navLinks[i];
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    }

    // Динамическая валидация форм
    const forms = document.querySelectorAll('form.needs-validation');

    for (let i = 0; i < forms.length; i++) {
        forms[i].addEventListener('submit', function (event) {
            if (!forms[i].checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            forms[i].classList.add('was-validated');
        });
    }
});

// Функция для показа уведомлений
function showNotification(message, type) {
    if (!type) {
        type = 'success';
    }

    const notification = document.createElement('div');
    notification.className = 'alert alert-' + type + ' notification';
    notification.textContent = message;
    notification.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px; animation: slideIn 0.3s ease;';

    document.body.appendChild(notification);

    setTimeout(function () {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(function () {
            notification.remove();
        }, 300);
    }, 3000);
}

// Анимации для уведомлений
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Валидация формы добавления клиента
document.addEventListener('DOMContentLoaded', function () {
    // Маска для телефона
    const phoneInputs = document.querySelectorAll('input[type="tel"]');

    phoneInputs.forEach(input => {
        input.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            // Убираем первую 7 если она есть
            if (value.startsWith('7')) {
                value = value.substring(1);
            }

            // Форматируем номер
            let formatted = '';
            if (value.length > 0) {
                formatted = '+7 (' + value.substring(0, 3);
            }
            if (value.length > 3) {
                formatted += ') ' + value.substring(3, 6);
            }
            if (value.length > 6) {
                formatted += '-' + value.substring(6, 8);
            }
            if (value.length > 8) {
                formatted += '-' + value.substring(8, 10);
            }

            e.target.value = formatted;
        });
    });

    // Валидация ИНН
    const innInputs = document.querySelectorAll('input[name="INN"]');

    innInputs.forEach(input => {
        input.addEventListener('blur', function () {
            const value = this.value.replace(/\D/g, '');
            if (value && !(value.length === 10 || value.length === 12)) {
                this.classList.add('is-invalid');
                this.setCustomValidity('ИНН должен содержать 10 или 12 цифр');
            } else {
                this.classList.remove('is-invalid');
                this.setCustomValidity('');
            }
        });
    });

    // Показ ошибок при отправке формы
    const forms = document.querySelectorAll('form[novalidate]');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const requiredFields = form.querySelectorAll('[required]');
            let isValid = true;
            const firstErrorField = [];

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('is-invalid');

                    // Запоминаем первое поле с ошибкой
                    if (firstErrorField.length === 0) {
                        firstErrorField.push(field);
                    }
                } else {
                    field.classList.remove('is-invalid');
                }
            });

            if (!isValid) {
                e.preventDefault();

                // Прокручиваем к первой ошибке
                if (firstErrorField.length > 0) {
                    firstErrorField[0].scrollIntoView({
                        behavior: 'smooth',
                        block: 'center'
                    });
                    firstErrorField[0].focus();
                }

                // Показываем уведомление
                showNotification(
                    'Заполните все обязательные поля, отмеченные *',
                    'danger'
                );
            }
        });
    });

    // Убираем подсветку ошибок при исправлении
    document.addEventListener('input', function (e) {
        if (e.target.hasAttribute('required')) {
            e.target.classList.remove('is-invalid');
        }
    });
});

// Функция показа уведомлений
function showNotification(message, type = 'success') {
    // Создаем контейнер для уведомлений если его нет
    let container = document.getElementById('notification-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999;';
        document.body.appendChild(container);
    }

    // Создаем уведомление
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} notification`;
    notification.innerHTML = `
        <div style="min-width: 300px; max-width: 400px;">
            ${message}
        </div>
    `;

    notification.style.cssText = 'margin-bottom: 10px; animation: slideIn 0.3s ease;';

    // Добавляем уведомление
    container.appendChild(notification);

    // Удаляем через 5 секунд
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 5000);
}