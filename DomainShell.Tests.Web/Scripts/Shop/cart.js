require(["el", "coco", "text!/Views/Shop/Cart"], function (el, coco, template) {
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
                    self.$context.pin("number").val(self.$params.number);                    

                    self.$context.pin("number").on("change", function (event) {
                        $.post(
                            "/api/shop/update",
                            {
                                customerId: "1",
                                cartItemId: self.$params.cartItemId,
                                number: event.target.value
                            })
                        .success(function (result) {
                            
                        }).fail(function (result) {
                            $("body").html(result.responseText);
                        });
                    });

                    self.$context.pin("remove").on("click", function () {
                        $.post(
                            "/api/shop/remove",
                            {
                                customerId: "1",
                                cartItemId: self.$params.cartItemId
                            })
                        .success(function (result) {                        
                            
                        }).fail(function (result) {
                            $("body").html(result.responseText);
                        });

                        self.$context().remove();
                    });
                }
            }
            
            self.$context("table tbody").empty();            
            $.post("/api/shop/getcartitems", { customerId: 1 }).success(function (cartItems) {
                cartItems.forEach(function (cartItem) {
                    var view = self.$coco({
                        model: tr,
                        params: cartItem
                    });

                    self.$context("table tbody").append(view.el);
                });
            }).fail(function (result) {
                $("body").html(result.responseText);
            });

            self.$context.pin("checkout").on("click", function () {
                $.post("/api/shop/getcartitems", { customerId: 1 }).success(function (cartItems) {
                    if (cartItems.length > 0)
                    {
                        location.href = "/shop/checkout"
                    } else {
                        alert("not item in cart");
                    }
                }).fail(function (result) {
                    $("body").html(result.responseText);
                });
                
            });
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   