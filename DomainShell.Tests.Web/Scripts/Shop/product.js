require(["el", "coco", "text!/Views/Shop/Product"], function (el, coco, template) {
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
                            "/api/shop/add",
                            {
                                customerId: "1",
                                productId: self.$params.productId,
                                number: self.$context.pin("number").val()
                            })
                        .success(function (result) {
                            location.href = "/cart/cart"
                        }).fail(function (result) {
                            $("body").html(result.responseText);
                        });
                    });
                }
            }

            self.$context("table tbody").empty();
            $.post("/api/shop/getproducts").success(function (products) {
                products.forEach(function (product) {
                    var view = self.$coco({
                        model: tr,
                        params: product
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