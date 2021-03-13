﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace TheLastTime.Shared.Components
{
    public partial class CheckBox
    {
        [Inject]
        JsInterop JsInterop { get; set; } = null!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public bool IsTriState { get; set; }

        [Parameter]
        public bool? Checked { get; set; }

        [Parameter]
        public EventCallback<bool?> CheckedChanged { get; set; }

        private bool internalChecked;

        private bool isIndeterminate;

        private ElementReference elementReference;

        private string elementId = Guid.NewGuid().ToString();

        private void SetInternalChecked()
        {
            internalChecked = Checked != false;
        }

        protected override void OnParametersSet()
        {
            SetInternalChecked();
        }

        private async Task SetIndeterminate()
        {
            bool indeterminate = Checked == null;

            if (isIndeterminate != indeterminate)
            {
                isIndeterminate = indeterminate;

                await JsInterop.SetElementProperty(elementReference, "indeterminate", indeterminate);
            }
        }

        //private async ValueTask SetElementProperty(ElementReference element, string property, object value)
        //{
        //    await jsRuntime.InvokeVoidAsync("setElementProperty", element, property, value);
        //}

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SetIndeterminate();
        }

        private async Task ChangeChecked()
        {
            if (IsTriState)
            {
                Checked = Checked switch
                {
                    false => true,
                    true => null,
                    null => false,
                };
            }
            else
            {
                Checked = !Checked;
            }

            await CheckedChanged.InvokeAsync(Checked);
        }

        private async Task OnChange(ChangeEventArgs e)
        {
            await ChangeChecked();

            SetInternalChecked();

            await SetIndeterminate();
        }
    }
}
