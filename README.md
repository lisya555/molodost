Вот HTML-файлы для вашей системы управления продукцией мебельной компании без стилей, полностью соответствующие вашему примеру:

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>Создание продукта</title>
</head>
<body>
    <div>
        <a href="AddProduct.html">Добавить продукт</a>
        <a href="GetProducts.html">Список продукции</a>
        <a href="UpdateProduct.html">Редактировать продукт</a>
        <a href="Workshops.html">Цеха производства</a>
    </div>
    
    <iframe name="frame" style="display: none;"></iframe>
    
    <form target="frame" action="http://localhost:5066/products/create" method="post">
        <h2>Создание нового продукта</h2>
        
        <input required placeholder="ID продукта" type="number" name="id"/><br/>
        
        <input required placeholder="Название" type="text" name="name"/><br/>
        
        <input required placeholder="Стиль" type="text" name="style"/><br/>
        
        <input required placeholder="Материал" type="text" name="material"/><br/>
        
        <input required placeholder="Цена" type="number" step="0.01" name="price"/><br/>
        
        <input required placeholder="ID цеха" type="number" name="workshopId"/><br/>
        
        <input type="submit" value="Создать продукт"/>
    </form>
</body>
</html>
```

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>Список продукции</title>
</head>
<body>
    <div>
        <a href="AddProduct.html">Добавить продукт</a>
        <a href="GetProducts.html">Список продукции</a>
        <a href="UpdateProduct.html">Редактировать продукт</a>
        <a href="Workshops.html">Цеха производства</a>
    </div>
    
    <table>
        <tr>
            <th>ID</th>
            <th>Название</th>
            <th>Стиль</th>
            <th>Материал</th>
            <th>Цена</th>
            <th>ID цеха</th>
        </tr>
    </table>

    <script>
        let table = document.querySelector("table");
        GetProducts();

        async function GetProducts()
        {
            let response = await fetch("http://localhost:5066/products",{method:"GET"});
            let products = await response.json();

            products.products.forEach(p => {
                let tr = document.createElement("tr");

                tr.append(CreateTd(p.id));
                tr.append(CreateTd(p.name));
                tr.append(CreateTd(p.style));
                tr.append(CreateTd(p.material));
                tr.append(CreateTd(p.price));
                tr.append(CreateTd(p.workshopId));

                table.append(tr)
            });
        }
        
        function CreateTd(data)
        {
            let td = document.createElement("td");
            td.append(data);
            return td;
        }
    </script>
</body>
</html>
```

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>Редактирование продукта</title>
</head>
<body>
    <div>
        <a href="AddProduct.html">Добавить продукт</a>
        <a href="GetProducts.html">Список продукции</a>
        <a href="UpdateProduct.html">Редактировать продукт</a>
        <a href="Workshops.html">Цеха производства</a>
    </div>
    
    <iframe name="frame" style="display: none;"></iframe>
    
    <form target="frame" action="http://localhost:5066/products/update" method="post">
        <h2>Редактирование продукта</h2>
        
        <input required placeholder="ID продукта" type="number" name="Id"/><br/>
        
        <input placeholder="Новое название" type="text" name="Name"/><br/>
        
        <input placeholder="Новый стиль" type="text" name="Style"/><br/>
        
        <input placeholder="Новый материал" type="text" name="Material"/><br/>
        
        <input placeholder="Новая цена" type="number" step="0.01" name="Price"/><br/>
        
        <input placeholder="Новый ID цеха" type="number" name="WorkshopId"/><br/>
        
        <input type="submit" value="Обновить продукт"/>
    </form>
</body>
</html>
```

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <title>Цеха производства</title>
</head>
<body>
    <div>
        <a href="AddProduct.html">Добавить продукт</a>
        <a href="GetProducts.html">Список продукции</a>
        <a href="UpdateProduct.html">Редактировать продукт</a>
        <a href="Workshops.html">Цеха производства</a>
    </div>
    
    <table>
        <tr>
            <th>ID цеха</th>
            <th>Название цеха</th>
        </tr>
    </table>

    <script>
        let table = document.querySelector("table");
        GetWorkshops();

        async function GetWorkshops()
        {
            let response = await fetch("http://localhost:5066/workshops",{method:"GET"});
            let workshops = await response.json();

            workshops.forEach(w => {
                let tr = document.createElement("tr");

                tr.append(CreateTd(w.id));
                tr.append(CreateTd(w.name));

                table.append(tr)
            });
        }
        
        function CreateTd(data)
        {
            let td = document.createElement("td");
            td.append(data);
            return td;
        }
    </script>
</body>
</html>
```

### Получить все продукты
GET http://localhost:5066/products
Accept: application/json

### Получить продукт по ID (например, ID 1)
GET http://localhost:5066/products?param=1
Accept: application/json

### Создать новый продукт
POST http://localhost:5066/products/create
Content-Type: application/json

{
    "id": 3,
    "name": "Классический шкаф",
    "style": "Классика",
    "material": "Красное дерево",
    "price": 35000,
    "workshopId": 2
}

### Обновить продукт
PUT http://localhost:5066/products/update
Content-Type: application/json

{
    "Id": 2,
    "Name": "Премиум кресло",
    "Style": "Классика",
    "Material": "Орех премиум",
    "Price": 22000,
    "WorkshopId": 2
}

### Получить список цехов
GET http://localhost:5066/workshops
Accept: application/json
