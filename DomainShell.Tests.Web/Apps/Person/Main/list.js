require(["el", "coco", "text!apps/person/main/list.html", "apps/person/parts/tr"], function (el, coco, template, tr) {
    var list = {
        node: template,
        ready: function () {
            var self = this;

            $.get("api/person/getall").success(function (persons) {
                for (var id in persons) {
                    view = self.$coco({
                        model: tr,
                        params: persons[id]
                    });

                    self.$context("table tbody").append(view.el);
                }
            }).fail(function (result) {                
                self.$context(".message").html(result.responseText);
            });

            self.$context("[name=output]").on("click", function () {
                location.href = "api/person/output";
            });
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   