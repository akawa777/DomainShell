define(["home/detailApp"], function (detailApp) {
    var listApp = {
        template: "",
        data: {
            id: "",            
            list: [],
            visible: false
        },
        create: function (render, event, item) {
            item.id = "";            
            item.visible = true;

            render();
        },
        detail: function (render, event, item) {
            var self = this;
            self.data.id = item.id;
            self.data.visible = true;

            render();
        },
        list: function (render) {
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

                render();
            }).fail(function (data) {
                alert("error!");
            });
        },
        detailApp: function(components, mold, item, options) {            
            var self = this;

            var list = function (render) {
                self.list(render);
            }

            mold(detailApp, { data: { id: self.data.id, visible: self.data.visible }, list: list });
        },
        init: function (render) {
            var self = this;

            $.get("/scripts/home/list.html").done(function (template) {
                self.template = template;
                self.list(render);
            });            
        }
    }    

    return listApp;
});