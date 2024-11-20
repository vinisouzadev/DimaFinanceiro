window.checkout = (publicStripeKey, sessionId) => {
    let stripe = Stripe(publicStripeKey);
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
};