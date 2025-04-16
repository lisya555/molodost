// Базовые настройки
const API_BASE_URL = 'http://localhost:5000/api';
let currentUser = null;

// Инициализация приложения
document.addEventListener('DOMContentLoaded', () => {
    setupNavigation();
    checkAuthStatus();
});

// Настройка навигации
function setupNavigation() {
    // Общие элементы
    document.querySelectorAll('[id$="-link"]').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const page = this.id.replace('-link', '');
            navigateTo(page);
        });
    });

    // Специальные обработчики
    document.getElementById('show-register')?.addEventListener('click', () => navigateTo('register'));
    document.getElementById('show-register-link')?.addEventListener('click', () => navigateTo('register'));
    document.getElementById('show-login-link')?.addEventListener('click', () => navigateTo('login'));
}

// Навигация между страницами
function navigateTo(page) {
    switch(page) {
        case 'home':
            window.location.href = 'index.html';
            break;
        case 'login':
            window.location.href = 'auth.html';
            break;
        case 'register':
            window.location.href = 'auth.html#register';
            break;
        case 'account':
            window.location.href = 'account.html';
            break;
        case 'reviews':
            window.location.href = 'reviews.html';
            break;
        case 'logout':
            logout();
            break;
        default:
            window.location.href = 'index.html';
    }
}

// Проверка статуса авторизации
async function checkAuthStatus() {
    const token = localStorage.getItem('token');
    if (token) {
        try {
            const response = await fetch(`${API_BASE_URL}/auth/me`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            
            if (response.ok) {
                currentUser = await response.json();
                updateUIForLoggedInUser();
            } else {
                logout();
            }
        } catch (error) {
            console.error('Auth check failed:', error);
        }
    }
}

// Обновление UI для авторизованного пользователя
function updateUIForLoggedInUser() {
    // Скрываем кнопки входа/регистрации
    document.getElementById('login-link')?.style.display = 'none';
    document.getElementById('register-link')?.style.display = 'none';
    
    // Показываем кнопки профиля и выхода
    document.getElementById('account-link')?.style.display = 'block';
    document.getElementById('logout-link')?.style.display = 'block';
    
    // Обновляем информацию в личном кабинете
    if (window.location.pathname.includes('account.html')) {
        document.getElementById('user-name').textContent = currentUser.name;
        document.getElementById('user-email').textContent = currentUser.email;
    }
}

// Выход из системы
function logout() {
    localStorage.removeItem('token');
    currentUser = null;
    window.location.href = 'index.html';
}

// Обработка форм
document.getElementById('loginForm')?.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });
        
        if (response.ok) {
            const data = await response.json();
            localStorage.setItem('token', data.token);
            window.location.href = 'account.html';
        } else {
            alert('Неверные учетные данные');
        }
    } catch (error) {
        console.error('Login error:', error);
        alert('Ошибка при входе');
    }
});

document.getElementById('registerForm')?.addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const name = document.getElementById('registerName').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;
    
    if (password !== confirmPassword) {
        alert('Пароли не совпадают');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, email, password })
        });
        
        if (response.ok) {
            alert('Регистрация успешна! Теперь вы можете войти.');
            window.location.href = 'auth.html';
        } else {
            const error = await response.json();
            alert(error.message || 'Ошибка при регистрации');
        }
    } catch (error) {
        console.error('Registration error:', error);
        alert('Ошибка при регистрации');
    }
});

// Инициализация страницы при загрузке
if (window.location.hash === '#register') {
    document.getElementById('login-form').style.display = 'none';
    document.getElementById('register-form').style.display = 'block';
}