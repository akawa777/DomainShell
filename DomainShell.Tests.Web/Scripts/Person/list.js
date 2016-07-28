require(["el", "coco", "text!/views/person/list", "person/tr"], function (el, coco, template, tr) {
    var list = {
        node: template,
        init: function() {
            var self = this;

            var dfd = $.Deferred();

            $.get("/api/person/getall").success(function (persons) {
                self.$components = {
                    tr: {
                        model: tr,
                        eachParams: persons
                    }
                }

                dfd.resolve();
            }).fail(function (result) {
                $("body").html(result.responseText);
            });

            return dfd.promise();
        },
        ready: function () {
            var self = this;

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