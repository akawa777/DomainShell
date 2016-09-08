require(["el", "coco", "text!/Views/Shop/History"], function (el, coco, template) {
    var list = {
        node: template,        
        ready: function () {
            var self = this;

            var tr = {
                node: self.$context("table tbody").html(),
                ready: function () {
                    var self = this;
                    self.$context.pin("paymentId").text(self.$params.paymentId);
                    self.$context.pin("paymentDate").text(self.$params.paymentDate);
                    self.$context.pin("shippingAddress").text(self.$params.shippingAddress);
                    self.$context.pin("paymentAmount").text(self.$params.paymentAmount);
                }
            }

            self.$context("table tbody").empty();
            $.post("/api/purchase/getpayments", { customerId: "1" }).success(function (paymentContents) {
                paymentContents.forEach(function (paymentContent) {
                    var view = self.$coco({
                        model: tr,
                        params: paymentContent
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