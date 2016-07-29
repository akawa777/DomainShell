require(["el", "coco", "text!/views/person/list2"], function (el, coco, template) {
    var list = {
        node: template,
        init: function() {
            var self = this;

            self.$components = {
                tr: {
                    model: {
                        ready: function () {
                            var self = this;
                            self.$context(".id").text(self.$params.Id);
                            self.$context(".name").text(self.$params.Name);
                        }
                    },
                    eachParams: [
                        { Id: 1, Name: "1_name" },
                        { Id: 2, Name: "2_name" },
                        { Id: 3, Name: "3_name" },
                    ]
                }
            }
        },
        ready: function () {
            var self = this;

            self.$context("[name=prev]").on("click", function () {
                var eachParams = [
                    { Id: 1, Name: "1_name" },
                    { Id: 2, Name: "2_name" },
                    { Id: 3, Name: "3_name" }
                ];

                self.$components.tr.reload(eachParams);
            });

            self.$context("[name=next]").on("click", function () {
                var eachParams = [
                    { Id: 4, Name: "6_name" },
                    { Id: 5, Name: "5_name" },
                    { Id: 6, Name: "4_name" }
                ];

                self.$components.tr.reload(eachParams);
            });

            self.$context("[name=append]").on("click", function () {
                var params = { Id: "append", Name: "append_name" };
                self.$components.tr.views.append(params);                
            });

            self.$context("[name=prepend]").on("click", function () {
                var params = { Id: "prepend", Name: "prepend_name" };
                self.$components.tr.views.prepend(params);                
            });

            self.$context("[name=remove]").on("click", function () {                
                self.$components.tr.views.remove();                
            });

            self.$context("[name=removeAll]").on("click", function () {                
                self.$components.tr.views.removeAll();
            });
        }

    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   