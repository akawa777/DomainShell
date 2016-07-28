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
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   