async function addToCart(createCheckoutButton, quantity, productId) {
    const createCheckoutButtonTypeIsCorrect = (typeof (createCheckoutButton) === 'boolean');
    const quantityTypeIsCorrect = Number.isInteger(quantity) && quantity >= 1;
    const productIdTypeIsCorrect = Number.isInteger(productId) && productId >= 1;
    // Note that the if part won't execute if quantity=0,
    // which avoids calling the backend.
    if (createCheckoutButtonTypeIsCorrect && quantityTypeIsCorrect && productIdTypeIsCorrect) {

        const jsonToSend = JSON.stringify({ "createCheckoutButton": createCheckoutButton, "quantity": quantity, "productId": productId });
        const request = new Request("/UserCart/Add",
            {
                method: "POST",
                headers:
                {
                    "Content-Type": "application/json",
                },
                body: jsonToSend,
            });

        try {
            const response = await fetch(request);
            if (!response.ok) {
                throw new Error(`Response status: ${response.status}`);
            }

            // update the cart in the HTML
            document.getElementById("cart").innerHTML = await response.text();
        }
        catch (error) {
            console.log("Error:", error);
        }
    }
    else {
        console.log("The following inputs of addToCart have incorrect types:");
        if (createCheckoutButtonTypeIsCorrect === false) console.log(`createCheckoutButton = ${createCheckoutButton}`);
        if (quantityTypeIsCorrect === false) console.log(`quantity = ${quantity}`);
        if (productIdTypeIsCorrect === false) console.log(`productId = ${productId}`);
    }
}

async function RemoveFromCart(createCheckoutButton, productId) {
    const createCheckoutButtonTypeIsCorrect = (typeof (createCheckoutButton) === 'boolean');
    const productIdTypeIsCorrect = Number.isInteger(productId) && productId >= 1;
    if (createCheckoutButtonTypeIsCorrect && productIdTypeIsCorrect) {
        const jsonToSend = JSON.stringify({ "createCheckoutButton": createCheckoutButton, "productId": productId });
        const request = new Request("/UserCart/RemoveSingleProduct",
            {
                method: "DELETE",
                headers:
                {
                    "Content-Type": "application/json",
                },
                body: jsonToSend,
            });
        try {
            const response = await fetch(request);
            if (!response.ok) {
                throw (new Error(`Response status: ${response.status}`));
            }

            // update the cart in the HTML
            document.getElementById("cart").innerHTML = await response.text();
        }
        catch (error) {
            console.log("Error:", error);
        }
    }
    else {
        console.log("The following inputs of RemoveFromCart have incorrect types:");
        if (createCheckoutButtonTypeIsCorrect === false) console.log(`createCheckoutButton = ${createCheckoutButton}`);
        if (productIdTypeIsCorrect === false) console.log(`productId = ${productId}`);
    }
}

async function RemoveCart() {
    const request = new Request("/Usercart/RemoveAllProducts",
        {
            method: "DELETE",
        }
    );
    try {
        const response = await fetch(request);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        // update the cart in the HTML
        document.getElementById("cart").innerHTML = await response.text();
    }
    catch (error) {
        console.log("Error:", error);
    }
}