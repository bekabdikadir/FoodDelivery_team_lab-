(() => {
    const sidebar = document.getElementById('cartSidebar');
    const summaryContainer = document.getElementById('cart-summary-container');
    const countBadge = document.getElementById('cart-item-count');
    const token = document.querySelector('#cart-antiforgery-form input[name="__RequestVerificationToken"]')?.value;
    const toast = document.getElementById('cart-toast');
    let toastTimer;

    if (!sidebar || !summaryContainer || !countBadge || !token) return;

    const showMessage = (message, failed = false) => {
        toast.textContent = message;
        toast.classList.toggle('is-error', failed);
        toast.classList.add('is-visible');
        window.clearTimeout(toastTimer);
        toastTimer = window.setTimeout(() => toast.classList.remove('is-visible'), 3000);
    };

    const refreshSummary = async () => {
        const response = await fetch(sidebar.dataset.summaryUrl, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        if (!response.ok) throw new Error('Не удалось загрузить корзину.');
        const template = document.createElement('template');
        template.innerHTML = await response.text();
        summaryContainer.replaceChildren(template.content.cloneNode(true));
        countBadge.textContent = summaryContainer.querySelector('[data-total-quantity]')?.dataset.totalQuantity ?? '0';
    };

    const post = async (url, fields) => {
        const body = new URLSearchParams(fields);
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
                'RequestVerificationToken': token,
                'X-Requested-With': 'XMLHttpRequest'
            },
            body
        });
        const result = await response.json();
        if (!response.ok || !result.success) throw new Error(result.message ?? 'Не удалось обновить корзину.');
        await refreshSummary();
        showMessage(result.message);
    };

    document.addEventListener('click', async event => {
        const addButton = event.target.closest('[data-cart-add]');
        const quantityButton = event.target.closest('[data-cart-quantity]');
        const removeButton = event.target.closest('[data-cart-remove]');

        try {
            if (addButton) {
                addButton.disabled = true;
                await post(sidebar.dataset.addUrl, {
                    restaurantId: addButton.dataset.restaurantId,
                    dishId: addButton.dataset.dishId
                });
                addButton.disabled = false;
            } else if (quantityButton) {
                const nextQuantity = Number(quantityButton.dataset.quantity) + Number(quantityButton.dataset.cartQuantity);
                if (nextQuantity < 1) return;
                await post(sidebar.dataset.quantityUrl, {
                    restaurantId: quantityButton.dataset.restaurantId,
                    dishId: quantityButton.dataset.dishId,
                    quantity: String(nextQuantity)
                });
            } else if (removeButton) {
                await post(sidebar.dataset.removeUrl, {
                    restaurantId: removeButton.dataset.restaurantId,
                    dishId: removeButton.dataset.dishId
                });
            }
        } catch (error) {
            if (addButton) addButton.disabled = false;
            showMessage(error.message, true);
        }
    });

    refreshSummary().catch(error => showMessage(error.message, true));
})();
