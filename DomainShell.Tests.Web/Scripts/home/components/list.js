define(["home/components/detail"], function (detail) {
    var listApp = {        
        template: "",
        data: {
            list: [],
            detail: {
                id: "",
                name: ""
            }
        },
        componetns: {
            detail: detail
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
        clickCreate: function (adapt, event, item) {
            var self = this;
            self.data.detail.id = "";

            adapt(function () {
                self.modal();
            });
        },
        clickDetail: function (adapt, event, item) {
            var self = this;
            self.data.detail.id = item.id;

            var self = this;
            adapt(function () {
                self.modal();
            });
        },
        loadList: function (adapt) {
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

                self.data.detail.id = "";                

                adapt();
            }).fail(function (data) {
                alert("error!");
            });
        },
        detailComponent: function(render, item, options) {            
            var self = this;

            var loadList = function (adapt) {
                self.loadList(adapt);
            }

            render(self.componetns.detail, { data: self.data.detail, loadList: loadList });
        },
        init: function (adapt) {
            var self = this;

            $.get("/scripts/home/templates/list.html?bust=v2").done(function (template) {
                self.template = template;
                self.loadList(adapt);
            });            
        }
    }    

    return listApp;
});