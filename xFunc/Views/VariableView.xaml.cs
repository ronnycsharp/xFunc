﻿// Copyright 2012-2014 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Maths.Expressions.Collections;
using xFunc.ViewModels;

namespace xFunc.Views
{

    public partial class VariableView : Window
    {

        private Processor processor;

        #region Commands

        public static RoutedCommand AddCommand = new RoutedCommand();
        public static RoutedCommand EditCommand = new RoutedCommand();
        public static RoutedCommand DeleteCommand = new RoutedCommand();
        public static RoutedCommand RefreshCommand = new RoutedCommand();

        #endregion

        public VariableView(Processor processor)
        {
            this.processor = processor;
            RefreshList();

            this.SourceInitialized += (o, args) => this.HideMinimizeAndMaximizeButtons();

            InitializeComponent();
        }

        private void RefreshList()
        {
            this.DataContext = processor.Parameters.Select(v => new VariableViewModel(v));
            var view = (CollectionView)CollectionViewSource.GetDefaultView(this.DataContext);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
        }

        #region Commands

        private void AddCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            AddVariableView view = new AddVariableView()
            {
                Owner = this
            };
            if (view.ShowDialog() == true)
            {
                processor.Parameters.Add(new Parameter(view.VariableName, view.Value, view.IsReadOnly ? ParameterType.ReadOnly : ParameterType.Normal));

                RefreshList();
            }
        }

        private void EditCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            var item = varList.SelectedItem as VariableViewModel;

            AddVariableView view = new AddVariableView(item)
            {
                Owner = this
            };
            if (view.ShowDialog() == true)
            {
                var variable = processor.Parameters.First(v => v.Key == view.VariableName);
                variable.Value = view.Value;
                variable.Type = view.IsReadOnly ? ParameterType.ReadOnly : ParameterType.Normal;

                RefreshList();
            }
        }

        private void EditCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            var item = varList.SelectedItem as VariableViewModel;

            args.CanExecute = item != null && item.Type == ParameterType.Normal;
        }

        private void DeleteCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            try
            {
                var selectedItem = varList.SelectedItem as VariableViewModel;
                processor.Parameters.Remove(selectedItem.Variable);

                RefreshList();
            }
            catch (ParameterIsReadOnlyException)
            {
            }
        }

        private void DeleteCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            var item = varList.SelectedItem as VariableViewModel;

            args.CanExecute = item != null && item.Type != ParameterType.Constant;
        }

        private void RefreshCommand_Executed(object o, ExecutedRoutedEventArgs args)
        {
            RefreshList();
        }

        #endregion

    }

}
