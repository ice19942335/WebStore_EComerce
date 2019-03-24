﻿Cart = {
    _properties: {
        // Ссылка на метод добавления товара в корзину
        addToCartLink: '',
        // Ссылка на получение представления корзины
        getCartViewLink: '',
        // Ссылка на удаление товара из корзины
        removeFromCartLink: '',
    },
    // Инициализация логики
    init: function (properties) {
        // Копируем свойства
        $.extend(Cart._properties, properties);
        // Инициализируем перехват событий
        Cart.initEvents();
    },
    // Перехват события нажатий
    initEvents: function () {
        // Кнопка «Добавить в корзину»
        $('a.callAddToCart').on("click", Cart.addToCart);
        // Кнопка «Удалить товар из корзины»
        $('.cart_quantity_delete').on('click', Cart.removeFromCart);
    },
    // Событие добавления товара в корзину
    addToCart: function (event) {
        var button = $(this);
        // Отменяем дефолтное действие
        event.preventDefault();
        // Получение идентификатора из атрибута
        var id = button.data('id');
        // Вызов метода контроллера
        $.get(Cart._properties.addToCartLink + '/' + id).done(function () {
            // Отображаем сообщение, что товар добавлен в корзину
            Cart.showToolTip(button);
            // В случае успеха – обновляем представление
            Cart.refreshCartView();
        }).fail(function () { console.log('addToCart error'); });
    },
    refreshCartView: function () {
        // Получаем контейнер корзины
        var container = $("#cartContainer");
        // Получение представления корзины
        $.get(Cart._properties.getCartViewLink).done(function (result) {
            // Обновление html
            container.html(result);
        }).fail(function () { console.log('refreshCartView error'); });
    },
    showToolTip: function (button) {
        // Отображаем тултип
        button.tooltip({
            title: "Добавлено в корзину"
        }).tooltip('show');
        // Дестроим его через 0.5 секунды
        setTimeout(function () {
            button.tooltip('destroy');
        }, 500);
    },
    removeFromCart: function (event) {
        var button = $(this);
        // Отменяем дефолтное действие
        event.preventDefault();
        // Получение идентификатора из атрибута
        var id = button.data('id');
        $.get(Cart._properties.removeFromCartLink + '/' + id).done(function () {
            button.closest('tr').remove();
            // В случае успеха – обновляем представление
            Cart.refreshCartView();
        }).fail(function () { console.log('addToCart error'); });
    }
}
