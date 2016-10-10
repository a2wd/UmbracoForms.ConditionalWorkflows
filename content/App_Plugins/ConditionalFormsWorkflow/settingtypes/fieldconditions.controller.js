angular.module("umbraco").controller("Chillies.SettingType.FieldConditionsController",
	function ($scope, $routeParams, pickerResource) {

	    function init() {

	        if (!$scope.setting.value) {
	            $scope.mappings = [];
	        } else {
	            $scope.mappings = JSON.parse($scope.setting.value);
	        }

	        var formId = $routeParams.id;

	        if (formId === -1 && $scope.model && $scope.model.fields) {

	        } else {

	            pickerResource.getAllFields($routeParams.id).then(function (response) {
	                $scope.fields = response.data;
	            });
	        }
	    }

	    $scope.addRoute = function () {
	        $scope.mappings.push({
	            field: "",
	            fieldValue: "",
	            emailRoute: ""
	        });
	    };

	    $scope.deleteRoute = function (index) {
	        $scope.mappings.splice(index, 1);
	        $scope.setting.value = JSON.stringify($scope.mappings);
	    };

	    $scope.stringifyValue = function () {
	        $scope.setting.value = JSON.stringify($scope.mappings);
	    };

	    init();

	});
