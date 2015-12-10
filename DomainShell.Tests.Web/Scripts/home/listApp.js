define(["home/detailApp"], function (detailApp) {
    var listApp = {        
        template: "",
        data: {
            id: "",            
            list: []
        },
        componetns: {
            detailApp: detailApp
        },
        modal: function(args) {

        },
        detailHook: function (el, adapt) {
            var self = this;

            self.modal = function (args) {
                if (args) {
                    $(el).modal(args);
                } else {
                    $(el).modal(args);
                }
            }
        },
        create: function (adapt, event, item) {
            item.id = "";

            var self = this;
            adapt(function () {
                self.modal();
            });
        },
        detail: function (adapt, event, item) {
            var self = this;
            self.data.id = item.id;            

            var self = this;
            adapt(function () {
                self.modal();
            });
        },
        list: function (adapt) {
            var self = this;

            self.modal("hide");

            $.ajax({
                url: "/home/list",
                method: "post"
            }).done(function (rtnData) {
                self.data.list = [];

                rtnData.forEach(function (item) {
                    self.data.list.push({
                        id: item.Id,
                        name: item.Name
                    });
                });

                self.data.id = "";
                self.data.visible = false;                

                adapt();
            }).fail(function (data) {
                alert("error!");
            });
        },
        detailApp: function(render, item, options) {            
            var self = this;

            var list = function (adapt) {
                self.list(adapt);
            }

            render(self.componetns.detailApp, { data: self.data, list: list });
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