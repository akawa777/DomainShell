require(["el", "coco", "text!/Views/Cart/Cart"], function (el, coco, template) {
    var list = {
        node: template,        
        ready: function () {
            var self = this;

            var tr = {
                node: self.$context("table tbody").html(),
                ready: function () {
                    var self = this;
                    self.$context.pin("cartItemId").text(self.$params.cartItemId);
                    self.$context.pin("productId").text(self.$params.productId);
                    self.$context.pin("productName").text(self.$params.productName);
                    self.$context.pin("price").text(self.$params.price);
                    self.$context.pin("number").text(self.$params.number);
                }
            }

            $.get("/api/cart/get?customerId=1").success(function (cartItems) {
                self.$context("table tbody").empty();
                cartItems.forEach(function (cartItem) {
                    var view = self.$coco({
                        model: tr,
                        params: {
                            cartItemId: cartItem.CartItemId,
                            productId: cartItem.ProductId,
                            productName: cartItem.ProductName,
                            price: cartItem.Price,
                            number: cartItem.Number
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