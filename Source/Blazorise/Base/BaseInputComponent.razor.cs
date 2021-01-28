﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for all the input component types.
    /// </summary>
    public abstract class BaseInputComponent<TValue> : BaseComponent, IValidationInput, IFocusableComponent
    {
        #region Members

        private Size size = Size.None;

        private bool readOnly;

        private bool disabled;

        /// <summary>
        /// Internal value for autofocus flag.
        /// </summary>
        private bool autofocus;

        private bool validationInitialized;

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            // For modals we need to make sure that autofocus is applied every time the modal is opened.
            if ( parameters.TryGetValue<bool>( nameof( Autofocus ), out var autofocus ) && this.autofocus != autofocus )
            {
                this.autofocus = autofocus;

                if ( autofocus )
                {
                    if ( ParentModal != null )
                    {
                        ParentModal.AddFocusableComponent( this );
                    }
                    else
                    {
                        ExecuteAfterRender( async () =>
                        {
                            await FocusAsync();
                        } );
                    }
                }
                else
                {
                    if ( ParentModal != null )
                    {
                        ParentModal.RemoveFocusableComponent( this );
                    }
                }
            }
        }

        protected void InitializeValidation()
        {
            if ( validationInitialized )
                return;

            // link to the parent component
            ParentValidation.InitializeInput( this );

            ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;

            validationInitialized = true;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidation != null )
                {
                    // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                    ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
                }

                if ( ParentModal != null )
                {
                    ParentModal.RemoveFocusableComponent( this );
                }
            }

            base.Dispose( disposing );
        }

        protected async void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs e )
        {
            DirtyClasses();
            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Handles the parsing of an input value.
        /// </summary>
        /// <param name="value">Input value to be parsed.</param>
        /// <returns>Returns the awaitable task.</returns>
        protected async Task CurrentValueHandler( string value )
        {
            var empty = false;

            if ( string.IsNullOrEmpty( value ) )
            {
                empty = true;
                CurrentValue = DefaultValue;
            }

            if ( !empty )
            {
                var result = await ParseValueFromStringAsync( value );

                if ( result.Success )
                {
                    CurrentValue = result.ParsedValue;
                }
            }

            // send the value to the validation for processing
            ParentValidation?.NotifyInputChanged();
        }

        protected abstract Task<ParseValue<TValue>> ParseValueFromStringAsync( string value );

        protected virtual string FormatValueAsString( TValue value )
            => value?.ToString();

        protected virtual object PrepareValueForValidation( TValue value )
            => value;

        /// <summary>
        /// Raises and event that handles the edit value of Text, Date, Numeric etc.
        /// </summary>
        /// <param name="value">New edit value.</param>
        protected abstract Task OnInternalValueChanged( TValue value );

        /// <inheritdoc/>
        public void Focus( bool scrollToElement = true )
        {
            _ = FocusAsync( scrollToElement );
        }

        /// <inheritdoc/>
        public async Task FocusAsync( bool scrollToElement = true )
        {
            await JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        [Inject] protected BlazoriseOptions Options { get; set; }

        /// <inheritdoc/>
        public virtual object ValidationValue => InternalValue;

        protected bool ParentIsFieldBody => ParentFieldBody != null;


        /// <summary>
        /// Returns the default value for the <typeparamref name="TValue"/> type.
        /// </summary>
        protected virtual TValue DefaultValue => default;

        /// <summary>
        /// Gets or sets the internal edit value.
        /// </summary>
        /// <remarks>
        /// The reason for this to be abstract is so that input components can have
        /// their own specialized parameters that can be binded(Text, Date, Value etc.)
        /// </remarks>
        protected abstract TValue InternalValue { get; set; }

        protected TValue CurrentValue
        {
            get => InternalValue;
            set
            {
                if ( !value.IsEqual( InternalValue ) )
                {
                    InternalValue = value;
                    _ = OnInternalValueChanged( value );
                }
            }
        }

        protected string CurrentValueAsString
        {
            get => FormatValueAsString( CurrentValue );
            set
            {
                _ = CurrentValueHandler( value );
            }
        }

        /// <summary>
        /// Sets the size of the input control.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Add the readonly boolean attribute on an input to prevent modification of the input’s value.
        /// </summary>
        [Parameter]
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                readOnly = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Set's the focus to the component after the rendering is done.
        /// </summary>
        [Parameter] public bool Autofocus { get; set; }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Input content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Occurs when the input box gains or loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }

        /// <summary>
        /// Occurs when the input box gains focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> FocusIn { get; set; }

        /// <summary>
        /// Occurs when the input box loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> FocusOut { get; set; }

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Parent validation container.
        /// </summary>
        [CascadingParameter] protected Validation ParentValidation { get; set; }

        /// <summary>
        /// Parent field body.
        /// </summary>
        [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

        /// <summary>
        /// Parent modal dialog.
        /// </summary>
        [CascadingParameter] protected Modal ParentModal { get; set; }

        #endregion
    }
}
