// we will use this array for adding items to our cart
var objProductArr = [];
var cartCounter = 0;

class Product {
    constructor(Id, Name, Cost, Image) {
        this.Id = Id;
        this.Name = Name;
        this.Cost = Cost;
        this.Image = Image;
    }
}

//
function initalizeProducts() {
    // Add Listener to View Cart button
    document.getElementById('viewBasketCheckOutButton').addEventListener('click', viewBasket);

    // Initialise Products
    const app = document.getElementById('products');

    var request = new XMLHttpRequest();
    request.open('GET', 'http://localhost:65390/api/products', true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                // add products to page
                var data = JSON.parse(this.responseText);
                for (var i = 0; i < data.length; i++) {
                    objProductArr.push(new Product(data[i].Id, data[i].Name, data[i].Cost, data[i].Image));

                    // The product is the header, product details container it will contain:
                    //      Image
                    //      Name
                    //      Cost
                    //      A button to add the products

                    var product = document.createElement('div');
                    product.setAttribute('class', 'product');

                    var productDetails = document.createElement('div');
                    productDetails.setAttribute('class', 'productDetails');

                    // Can't find any decent images, so not adding it for now
                    var productImage = document.createElement('div');
                    productImage.setAttribute('class', 'productImage');
                    //productImage.appendChild(document.createTextNode(data[i].Image));

                    // Name
                    var productName = document.createElement('h3');
                    productName.setAttribute('class', 'productName');
                    productName.appendChild(document.createTextNode(data[i].Name));

                    // Cost
                    var productCost = document.createElement('div');
                    productCost.setAttribute('class', 'productCost')
                    productCost.appendChild(document.createTextNode('$' + data[i].Cost));

                    // Add to cart button
                    var productAddToCart = document.createElement('a');
                    productAddToCart.setAttribute('class', 'buttonAddoRemove');
                    productAddToCart.setAttribute('onclick', 'addToCart(' + i + ')');
                    productAddToCart.appendChild(document.createTextNode('Add to Cart'));

                    productDetails.appendChild(productName);
                    productDetails.appendChild(productCost);
                    productDetails.appendChild(productAddToCart);
                    product.appendChild(productImage);
                    product.appendChild(productDetails);
                    app.appendChild(product);

                }

            }
            else {
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
                document.getElementById('errorMessage').style.display = 'block';
            }
        }
    }
    request.send();
}

// Adding product to cart
function addToCart(ProductId) {
    var data = JSON.stringify({ "Id": ++cartCounter, "Prod": objProductArr[ProductId] });
    var request = new XMLHttpRequest();
    request.open('POST', 'http://localhost:65390/api/Basket', true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                console.log(this.responseText);
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }

    }
    request.send(data);

}
// This is for making sure that there's products in the cart before viewing the cart
function viewBasket() {
    var request = new XMLHttpRequest();
    request.open('GET', 'http://localhost:65390/api/Basket', true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                window.location.href = "Views/ShoppingCart.html"
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }
    }
    request.send();
}