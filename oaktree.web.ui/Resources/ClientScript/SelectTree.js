jQuery(document).ready(function($) {
	$.each(OakTree.Web.UI.WebControls.SelectTree, function(i, control) {

		var keyName = control.keyName || 'key';
		var valuesName = control.valuesName || 'values';

		var selectTree = $('#' + control.controlID);
		var selects = selectTree.find('select');
		var addButton = selectTree.find('.addButton');
		var resultsList = selectTree.find('.resultsList');
		var parentSelect = selects.eq(0);
		var childSelect = selects.eq(1);
		var valueField = selectTree.find('input[type="hidden"]');

		var selectedValues = OakTree.Utilities.deserialize(valueField.val()) || [];

		addButton.click(function(e) {

			e.preventDefault();
			addValue();
			return false;
		});

		parentSelect.change(function() {
			buildChildSelect();
		});


		resultsList.find('a').live('click', function(e) {
			e.preventDefault();
			var el = $(this);
			if (el.parent().hasClass('childSelect')) {

				var parentName = el.parent().parent().parent().find('>.itemValue').text();
				var childName = el.parent().find('>.itemValue').text();

				var item = findByKey(parentName);
				item[valuesName].splice($.inArray(childName, item[valuesName]), 1);
				if (item[valuesName].length < 1) deleteByKey(parentName);
			}
			else {
				deleteByKey(el.parent().find('>.itemValue').text());
			}
			buildList();
			buildChildSelect();
		});

		var findByKey = function(key) {
			for (var i = 0; i < selectedValues.length; i++) {
				if (selectedValues[i][keyName] == key) return selectedValues[i];
			}
			return null;
		}
		var deleteByKey = function(key) {
			for (var i = 0; i < selectedValues.length; i++) {
				if (selectedValues[i][keyName] == key) {
					selectedValues.splice(i, 1);
				}
			}
		}

		//comparator function
		var sortVByKeyName = function(a, b) {
			var x = a[keyName].toLowerCase();
			var y = b[keyName].toLowerCase();
			return ((x < y) ? -1 : ((x > y) ? 1 : 0));
		}

		var addValue = function() {


			var childSelectedItems = childSelect.find('option:selected')
			var parentSelectedItems = parentSelect.find('option:selected');
			if (!parentSelect.val() || !childSelectedItems.length) return;

			$.each(parentSelectedItems, function(i, parentSelectedItem) {

				var parentSelectedValue = $(this).val();

				var item = findByKey(parentSelectedValue);

				if (!item) {
					item = {};
					item[keyName] = parentSelectedValue;
					item[valuesName] = [];
					selectedValues.push(item);
				}

				childSelectedItems.each(function() {
					var value = $(this).val();
					$(this).css('color', '#ddd').hide();
					if ($.inArray(value, item[valuesName]) == -1) {
						item[valuesName].push(value);
					}
				});
			});
			childSelect.focus();
			childSelect.val('');
			buildList();

		};

		var buildChildSelect = function() {
			var selectedItemName = parentSelect.val();
			var item = findByKey(selectedItemName);
			if (!item) {
				childSelect.find('option').css('color', '#000').show();
				childSelect.focus();
			}

			else {
				childSelect.find('option').each(function() {
					if ($.inArray($(this).val(), item[valuesName]) != -1) {
						$(this).css('color', '#ddd').hide();
					}
					else {
						$(this).css('color', '#000').show();
					}
				});
				childSelect.focus();
			}
		};

		var buildList = function() {
			selectedValues.sort(sortVByKeyName);

			resultsList.empty();

			$.each(selectedValues, function(i, item) {

				var li = $('<li class="parentSelect"><span class="itemValue">' + item[keyName] + '</span> <a href="#">remove</a></li>');
				var ul = $('<ul></ul>');
				$.each(item[valuesName], function(j, value) {

					ul.append('<li class="childSelect"><span class="itemValue">' + value + '</span> <a href="#">remove</a></li>');
				});

				li.append(ul);
				//alert(resultsList);
				resultsList.append(li);
			});

			valueField.val(OakTree.Utilities.serialize(selectedValues));
		};
		buildList();
		buildChildSelect();
	});

});
