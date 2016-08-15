require(["el", "coco", "text!/Views/Cart/ProductList"], function (el, coco, template) {
    var list = {
        node: template,        
        ready: function () {
            var self = this;

            var tr = {
                node: self.$context("table tbody").html(),
                ready: function () {
                    var self = this;
                    self.$context.pin("productId").text(self.$params.productId);
                    self.$context.pin("productName").text(self.$params.productName);
                    self.$context.pin("price").text(self.$params.price);

                    self.$context.pin("buy").on("click", function () {
                        $.post(
                            "/api/cart/add",
                            {
                                customerId: "1",
                                productId: self.$params.productId,
                                number: 1
                            })
                        .success(function (result) {

                        })
                    });
                }
            }

            $.get("/api/cart/getproducts").success(function (products) {
                self.$context("table tbody").empty();
                products.forEach(function (product) {
                    var view = self.$coco({
                        model: tr,
                        params: {
                            productId: product.ProductId,
                            productName: product.ProductName,
                            price: product.Price
                        }
                    });

                    self.$context("table tbody").append(view.el);
                });
            }).fail(function (result) {
                $("body").html(result.responseText);
            });
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   