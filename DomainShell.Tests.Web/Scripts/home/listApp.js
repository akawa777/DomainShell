define(["home/detailApp"], function (detailApp) {
    var listApp = {
        template: "",
        data: {
            id: "",            
            list: [],
            visible: false
        },
        create: function (adapt, event, item) {
            item.id = "";            
            item.visible = true;

            adapt();
        },
        detail: function (adapt, event, item) {
            var self = this;
            self.data.id = item.id;
            self.data.visible = true;

            adapt();
        },
        list: function (adapt) {
            var self = this;

            $.ajax({
                url: "/home/list",
                method: "post"
            }).done(function (rtnData) {
                var list = [];

                rtnData.forEach(function (item) {
                    list.push({
                        id: item.Id,
                        name: item.Name
                    });
                });

                self.data.list = list;

                self.data.id = "";
                self.data.visible = false;

                adapt();
            }).fail(function (data) {
                alert("error!");
            });
        },
        detailApp: function(components, render, item, options) {            
            var self = this;

            var list = function (adapt) {
                self.list(adapt);
            }

            render(detailApp, { data: self.data, list: list });
        },
        init: function (adapt) {
            var self = this;

            $.get("/scripts/home/list.html?bust=v2").done(function (template) {
                self.template = template;
                self.list(adapt);
            });            
        }
    }    

    return listApp;
});