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