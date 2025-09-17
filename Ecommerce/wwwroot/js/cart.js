async function addToCart(quantity, productId) {
    const quantityTypeIsCorrect = Number.isInteger(quantity) && quantity >= 1;
    const productIdTypeIsCorrect = Number.isInteger(productId) && productId >= 1;
    // Note that the if part won't execute if quantity=0,
    // which avoids calling the backend.
    if (quantityTypeIsCorrect && productIdTypeIsCorrect)
    {
        
        const jsonToSend = JSON.stringify({"quantity": quantity,"productId": productId});
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
        if (quantityTypeIsCorrect == false) console.log(`quantity = ${quantity}`);
        if (productIdTypeIsCorrect == false) console.log(`productId = ${productId}`);
    }
}

async function RemoveFromCart(productId) {
    const productIdTypeIsCorrect = Number.isInteger(productId) && productId >= 1;
    if (productIdTypeIsCorrect) {
        const request = new Request(`/UserCart/RemoveSingleProduct/${productId}`,
            {
                method: "DELETE",
            }
        );
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