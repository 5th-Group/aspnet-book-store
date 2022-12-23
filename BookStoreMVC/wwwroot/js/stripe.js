// const stripe = Stripe(
//   "pk_test_51MD0ypEOR35QfwSjJIwxvAAlp8XfwBcLxMhw4kjg7vI5HpeoICAsiKBBjEEa6YZmP9vYvDV0ojbXM15WVahyPmGu00Ns31efOE"
// );

// let elements;
// var payment_intent = {};
// var base_url = window.location.origin;

// initialize();

// document
//   .querySelector("#payment-form")
//   .addEventListener("submit", handleSubmit);

// // Fetches a payment intent and captures the client secret
// async function initialize() {
//   const response = await fetch("create-payment-intent", {
//     method: "POST",
//     headers: { "Content-Type": "application/json" },
//   });

//   payment_intent = await response.json();

//   const { clientSecret } = payment_intent;

//   const appearance = {
//     theme: "stripe",
//   };
//   elements = stripe.elements({ appearance, clientSecret });

//   const paymentElementOptions = {
//     layout: "tabs",
//   };

//   const paymentElement = elements.create("payment", paymentElementOptions);
//   paymentElement.mount("#payment-element");
// }

// async function handleSubmit(e) {
//   e.preventDefault();
//   setLoading(true);
//   const { paymentIntent_id } = payment_intent;

//   const { error } = await stripe.confirmPayment({
//     elements,
//     confirmParams: {
//       return_url: `${base_url}/payments/success`,
//     },
//   });

//   if (error) {
//     window.location.replace(
//       `${base_url}/payments/failed?paymentIntent_id=${paymentIntent_id}`
//     );
//   }

//   setLoading(false);
// }

// // ------- UI helpers -------

// function showMessage(messageText) {
//   const messageContainer = document.querySelector("#payment-message");

//   messageContainer.classList.remove("hidden");
//   messageContainer.textContent = messageText;

//   setTimeout(function () {
//     messageContainer.classList.add("hidden");
//     messageText.textContent = "";
//   }, 4000);
// }

// // Show a spinner on payment submission
// function setLoading(isLoading) {
//   if (isLoading) {
//     // Disable the button and show a spinner
//     document.querySelector("#submit").disabled = true;
//     document.querySelector("#spinner").classList.remove("hidden");
//     document.querySelector("#button-text").classList.add("hidden");
//   } else {
//     document.querySelector("#submit").disabled = false;
//     document.querySelector("#spinner").classList.add("hidden");
//     document.querySelector("#button-text").classList.remove("hidden");
//   }
// }

// This is your test publishable API key.
const stripe = Stripe(
  "pk_test_51MD0ypEOR35QfwSjJIwxvAAlp8XfwBcLxMhw4kjg7vI5HpeoICAsiKBBjEEa6YZmP9vYvDV0ojbXM15WVahyPmGu00Ns31efOE"
);

// The items the customer wants to buy

let elements;
var payment_intent = {};
var base_url = window.location.origin;

initialize();

document
  .querySelector("#payment-form")
  .addEventListener("submit", handleSubmit);

// Fetches a payment intent and captures the client secret
async function initialize() {
  const response = await fetch("/create-payment-intent", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
  });

  payment_intent = await response.json();

  const { clientSecret } = payment_intent;

  const appearance = {
    theme: "stripe",
  };
  elements = stripe.elements({ appearance, clientSecret });

  const paymentElementOptions = {
    layout: "tabs",
  };

  const paymentElement = elements.create("payment", paymentElementOptions);
  paymentElement.mount("#payment-element");
}

async function handleSubmit(e) {
  e.preventDefault();
  setLoading(true);
  const { paymentIntent_id } = payment_intent;

  const { error } = await stripe.confirmPayment({
    elements,
    confirmParams: {
      return_url: `${base_url}/checkout/success`,
    },
  });

  if (error) {
    window.location.replace(
      `${base_url}/checkout/failed?paymentIntent_id=${paymentIntent_id}`
    );
  }

  setLoading(false);
}

// Fetches the payment intent status after payment submission

// ------- UI helpers -------

function showMessage(messageText) {
  const messageContainer = document.querySelector("#payment-message");

  messageContainer.classList.remove("hidden");
  messageContainer.textContent = messageText;

  setTimeout(function () {
    messageContainer.classList.add("hidden");
    messageText.textContent = "";
  }, 4000);
}

// Show a spinner on payment submission
function setLoading(isLoading) {
  if (isLoading) {
    // Disable the button and show a spinner
    document.querySelector("#submit").disabled = true;
    document.querySelector("#spinner").classList.remove("hidden");
    document.querySelector("#button-text").classList.add("hidden");
  } else {
    document.querySelector("#submit").disabled = false;
    document.querySelector("#spinner").classList.add("hidden");
    document.querySelector("#button-text").classList.remove("hidden");
  }
}
