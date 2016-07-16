﻿define(["coco", "text!templates/person/tr.html"], function (coco, template) {
    var tr = {
        node: template,
        ready: function () {
            var self = this;

            self.$context.pin("id", "a").attr("href", "/person/detail/" + self.$params.Id);
            self.$context.pin("id", "a").text(self.$params.Id);
            self.$context.pin("name").text(self.$params.Name);

            this.messge = "complete";
        },
        messge: "",
        info: function () {
            alert("tr");
        }
    }

    coco.container.set("tr", tr);

    return tr;
});