define(["text!dashboard/templates/system.html?", "Chart"], function (template, Chart) {
    var system = {
        template: template,
        data: {
            datasets: [
                {
                    value: 300,
                    color: "#F7464A",
                    highlight: "#FF5A5E",
                    label: "Red"
                },
                {
                    value: 50,
                    color: "#46BFBD",
                    highlight: "#5AD3D1",
                    label: "Green"
                },
                {
                    value: 100,
                    color: "#FDB45C",
                    highlight: "#FFC870",
                    label: "Yellow"
                }
            ]
        },
        canvas: function (el) {
            var self = this;

            self.Doughnut = function () {
                var ctx = el.getContext("2d");
                var myDoughnutChart = new Chart(ctx).Doughnut(self.data.datasets, {
                    responsive: true
                });
            }
            
        },
        init: function (adapt, options) {
            var self = this;

            adapt(function () {
                self.Doughnut();
            });
        }
    }

    return system;
});