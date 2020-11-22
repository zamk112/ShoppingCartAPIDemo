function initialiseShoppingCart() {
    // Add listener for onclick for check out button
    document.getElementById('viewBasketCheckOutButton').addEventListener('click', checkOut);

    // Initialise the shopping cart and render it as a table.
    const table = document.getElementById('shoppingCartTable');

    var request = new XMLHttpRequest();
    request.open('GET', 'http://localhost:65390/api/Basket', true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                data = JSON.parse(this.responseText);

                for (var i = 0; i < data.length; i++) {
                    var row = document.createElement('tr');

                    // Remove from Checkout button
                    var cartDeleteCell = document.createElement('td');
                    var deleteOptionCellLink = document.createElement('a');
                    deleteOptionCellLink.setAttribute('class', 'buttonAddoRemove');
                    deleteOptionCellLink.setAttribute('onclick', 'updateItemCheckOut(' + data[i].Id + ', this' + ')');
                    deleteOptionCellLink.appendChild(document.createTextNode(data[i].Id));
                    cartDeleteCell.appendChild(deleteOptionCellLink);
                    row.appendChild(cartDeleteCell);

                    // Name
                    var cartNameCell = document.createElement("td");
                    var cartNameCellText = document.createTextNode(data[i].Prod.Name);
                    cartNameCell.appendChild(cartNameCellText);
                    row.appendChild(cartNameCell);

                    // Cost
                    var cartCostCell = document.createElement("td");
                    var cartCostCellText = document.createTextNode(data[i].Prod.Cost);
                    cartCostCell.appendChild(cartCostCellText);
                    row.appendChild(cartCostCell);

                    table.appendChild(row);
                }
                getShippingTotal('create');
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }
    }
    request.send();
    
}

// when the user clicks on the cell on the first columns; this function will remove the row from the table
function updateItemCheckOut(Id, row) {
    const table = document.getElementById('shoppingCartTable');
    var request = new XMLHttpRequest();
    var url = 'http://localhost:65390/api/Basket/' + Id;
    request.open('PUT', url, true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                if (this.status == 200) {
                    // get row num
                    var i = row.parentNode.parentNode.rowIndex;
                    // delete row from table
                    table.deleteRow(i);
                    // update
                    getShippingTotal('update');
                }
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }
    }

    request.send();
}

function getShippingTotal(Option) {
    var request = new XMLHttpRequest();
    request.open('GET', 'http://localhost:65390/api/Basket/getShippingCost/', true);
    request.setRequestHeader("Content-Type", "text/plain; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                var data = this.responseText;
                // Either create the totals section or update the numbers
                if (Option == 'create') {
                    const table = document.getElementById('shoppingCartTable');
                    var row = document.createElement('tr');

                    // Need to create delete cell but with no data
                    var cartDeleteCell = document.createElement("td");
                    cartDeleteCell.appendChild(document.createTextNode(''));
                    row.appendChild(cartDeleteCell);

                    // Add text in Name column
                    var cartNameCell = document.createElement("td");
                    cartNameCell.setAttribute('Id', 'nameShippingCost');
                    var cartNameCellText = document.createTextNode('Shipping Cost');
                    cartNameCell.appendChild(cartNameCellText);
                    row.appendChild(cartNameCell);

                    // Cost
                    // Also use the JSONKeyVal for styling the cells
                    var cartCostCell = document.createElement("td");
                    cartCostCell.setAttribute('Id', 'shippingCost');         // Set the attribute as JSON key value because need to update the values after deletion
                    var cartCostCellText = document.createTextNode(data);
                    cartCostCell.appendChild(cartCostCellText);
                    row.appendChild(cartCostCell);

                    table.appendChild(row);
                }
                else if (Option == 'update') {
                    var cell = document.getElementById('shippingCost');
                    cell.replaceChild(document.createTextNode(data), cell.childNodes[0]);
                }
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }
    }

    request.send();
}


function checkOut() {
    var request = new XMLHttpRequest();
    request.open('POST', 'http://localhost:65390/api/Basket/checkOut/', true);
    request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status >= 200 && this.status < 400) {
                // Once the checkout button is pressed; user can't go back
                location.replace('CheckOutCompletion.html');
            }
            else {
                document.getElementById('errorMessage').style.display = 'block';
                document.getElementById('errorMessage').appendChild(document.createTextNode(this.responseText));
            }
        }
    }
    request.send();
}