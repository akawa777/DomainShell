require(["el", "coco", "text!/views/person/list", "person/tr"], function (el, coco, template, tr) {
    var list = {
        node: template,        
        ready: function () {
            var self = this;

            $.get("/api/person/getall").success(function (persons) {
                self.$context("table tbody").empty();
                persons.forEach(function (person) {
                    var view = self.$coco({
                        model: tr,
                        params: person
                    });

                    self.$context("table tbody").append(view.el);
                });
            }).fail(function (result) {
                $("body").html(result.responseText);
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