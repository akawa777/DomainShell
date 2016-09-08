require(["el", "coco", "text!/Views/Cart/Payment"], function (el, coco, template) {
    var list = {
        node: template,        
        ready: function () {
            var self = this;

            var tr = {
                node: self.$context.pin("cart", "tbody").html(),
                ready: function () {
                    var self = this;
                    self.$context.pin("cartItemId").text(self.$params.cartItemId);
                    self.$context.pin("productId").text(self.$params.productId);
                    self.$context.pin("productName").text(self.$params.productName);
                    self.$context.pin("price").text(self.$params.price);
                    self.$context.pin("number").text(self.$params.number);
                }
            }
            
            var totalPrice = 0;
            self.$context.pin("cart", "tbody").empty();
            $.post("/api/cart/getcartitems", { customerId: 1 }).success(function (cartItems) {
                cartItems.forEach(function (cartItem) {
                    var view = self.$coco({
                        model: tr,
                        params: cartItem
                    });

                    totalPrice += cartItem.price;

                    self.$context.pin("cart", "tbody").append(view.el);
                });

                self.$context.pin("totalPrice").text(totalPrice);
            }).fail(function (result) {
                $("body").html(result.responseText);
            });

            $.post("/api/cart/getcustomer", { customerId: "1" }).success(function (customer) {
                self.$context.pin("shippingAddress").val(customer.address);
                self.$context.pin("creditCardNo").val(customer.creditCardNo);
                self.$context.pin("creditCardHolder").val(customer.creditCardHolder);
                self.$context.pin("creditCardExpirationDate").val(customer.creditCardExpirationDate);
            }).fail(function (result) {
                $("body").html(result.responseText);
            });

            $.post("/api/cart/getpaymentamountinfo", { customerId: "1" }).success(function (info) {
                self.$context.pin("totalPrice").text(info.totalPrice);
                self.$context.pin("postage").text(info.postage);
                self.$context.pin("tax").text(info.tax);
                self.$context.pin("paymentAmount").text(info.paymentAmount);
            }).fail(function (result) {
                $("body").html(result.responseText);
            });

            self.$context.pin("checkout").on("click", function () {
                if (window.confirm("do you checkout ?")) {
                    var payment = {
                        customerId: "1",
                        creditCardNo: self.$context.pin("creditCardNo").val(),
                        creditCardHolder: self.$context.pin("creditCardHolder").val(),
                        creditCardExpirationDate: self.$context.pin("creditCardExpirationDate").val(),
                        shippingAddress: self.$context.pin("shippingAddress").val()
                    }

                    $.post("/api/cart/checkout", payment).success(function () {
                        alert("completed checkout.");
                        location.href = "/Purchase/history"
                    }).fail(function (result) {
                        $("body").html(result.responseText);
                    });
                }                
            });
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   