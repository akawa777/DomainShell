require(["el", "coco", "text!/views/person/list2"], function (el, coco, template) {
    var persons = [
                { Id: 1, Name: "1_name" },
                { Id: 2, Name: "2_name" },
                { Id: 3, Name: "3_name" },
                { Id: 4, Name: "4_name" },
                { Id: 5, Name: "5_name" },
                { Id: 6, Name: "6_name" }
    ];

    var list = {
        node: template,        
        ready: function () {
            var self = this;

            var tr = {
                node: self.$context("table tbody").html(),
                ready: function () {
                    var self = this;
                    self.$context(".id input").val(self.$params.Id);
                    self.$context(".name input").val(self.$params.Name);

                    self.$context("[name=append]").on("click", function () {
                        var view = self.$coco({
                            model: self,
                            params: { Id: "", Name: "append" }
                        });

                        self.$context().after(view.el);
                    });

                    self.$context("[name=prepend]").on("click", function () {
                        var view = self.$coco({
                            model: self,
                            params: { Id: "", Name: "prepend" }
                        });

                        self.$context().before(view.el);
                    });

                    self.$context("[name=remove]").on("click", function () {
                        self.$context().remove();
                    });
                }
            }

            var showList = function (persons) {
                self.$context("table tbody").empty();
                persons.forEach(function (person) {
                    var view = self.$coco({
                        model: tr,
                        params: person
                    });

                    self.$context("table tbody").append(view.el);
                });
            }

            showList(persons.slice(0, 3));            

            self.$context("[name=new]").on("click", function () {
                var view = self.$coco({
                    model: tr,
                    params: { Id: "", Name: "new" }
                });

                self.$context("table tbody").append(view.el);
            });

            self.$context("[name=prev]").on("click", function () {
                showList(persons.slice(0, 3));
            });

            self.$context("[name=next]").on("click", function () {
                showList(persons.slice(3, persons.length));
            });
        }
    }

    var view = coco({
        model: list
    });

    $(el).replaceWith(view.el);
});   