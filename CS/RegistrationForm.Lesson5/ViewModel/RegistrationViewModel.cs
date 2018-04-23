﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using RegistrationForm.Common;
using RegistrationForm.DataModel;
using System;
using System.ComponentModel;
using System.Windows;

namespace RegistrationForm.ViewModel {
    [POCOViewModel]
    public class RegistrationViewModel : IDataErrorInfo {
        public static RegistrationViewModel Create() {
            return ViewModelSource.Create(() => new RegistrationViewModel());
        }
        protected RegistrationViewModel() {
            MinBirthday = new DateTime(DateTime.Now.Year - 100, 12, 31);
            MaxBirthday = new DateTime(DateTime.Now.Year - 1, 12, 31);
            if(this.IsInDesignMode())
                InitializeInDesignMode();
            else InitializeInRuntime();
        }
        void InitializeInDesignMode() {
            FirstName = "John";
            LastName = "Smith";
            Email = "John.Smith@JohnSmithMail.com";
            Password = "Password";
            ConfirmPassword = "Password";
            Birthday = new DateTime(1980, 1, 1);
            Gender = 1;
        }
        void InitializeInRuntime() {
            Birthday = null;
            Gender = -1;
        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public virtual DateTime? Birthday { get; set; }
        public virtual DateTime MinBirthday { get; set; }
        public virtual DateTime MaxBirthday { get; set; }
        public virtual int Gender { get; set; }

        public void AddEmployee() {
            string error = EnableValidationAndGetError();
            if(error != null) {
                OnValidationFailed(error);
                return;
            }
            EmployeesModelHelper.AddNewEmployee(FirstName, LastName, Email, Password, Birthday.Value, Gender);
            OnValidationSucceeded();
        }
        protected void OnPasswordChanged() {
            this.RaisePropertyChanged(x => x.ConfirmPassword);
        }
        void OnValidationSucceeded() {
            this.GetService<IMessageBoxService>().Show("Registration succeeded", "Registration Form", MessageBoxButton.OK);
        }
        void OnValidationFailed(string error) {
            this.GetService<IMessageBoxService>().Show("Registration failed. " + error, "Registration Form", MessageBoxButton.OK);
        }

        bool allowValidation = false;
        string EnableValidationAndGetError() {
            allowValidation = true;
            string error = ((IDataErrorInfo)this).Error;
            if(!string.IsNullOrEmpty(error)) {
                this.RaisePropertiesChanged();
                return error;
            }
            return null;
        }
        string IDataErrorInfo.Error {
            get {
                if(!allowValidation) return null;
                IDataErrorInfo me = (IDataErrorInfo)this;
                string error =
                    me[BindableBase.GetPropertyName(() => FirstName)] +
                    me[BindableBase.GetPropertyName(() => LastName)] +
                    me[BindableBase.GetPropertyName(() => Email)] +
                    me[BindableBase.GetPropertyName(() => Password)] +
                    me[BindableBase.GetPropertyName(() => ConfirmPassword)] +
                    me[BindableBase.GetPropertyName(() => Birthday)] +
                    me[BindableBase.GetPropertyName(() => Gender)];
                if(!string.IsNullOrEmpty(error))
                    return "Please check inputted data.";
                return null;
            }
        }
        string IDataErrorInfo.this[string columnName] {
            get {
                if(!allowValidation) return null;
                string firstNameProp = BindableBase.GetPropertyName(() => FirstName);
                string lastNameProp = BindableBase.GetPropertyName(() => LastName);
                string emailProp = BindableBase.GetPropertyName(() => Email);
                string passwordProp = BindableBase.GetPropertyName(() => Password);
                string confirmPasswordProp = BindableBase.GetPropertyName(() => ConfirmPassword);
                string birthdayProp = BindableBase.GetPropertyName(() => Birthday);
                string genderProp = BindableBase.GetPropertyName(() => Gender);
                if(columnName == firstNameProp) 
                    return RequiredValidationRule.GetErrorMessage(firstNameProp, FirstName);
                else if(columnName == lastNameProp)
                    return RequiredValidationRule.GetErrorMessage(lastNameProp, LastName);
                else if(columnName == emailProp)
                    return RequiredValidationRule.GetErrorMessage(emailProp, Email);
                else if(columnName == passwordProp)
                    return RequiredValidationRule.GetErrorMessage(passwordProp, Password);
                else if(columnName == confirmPasswordProp) {
                    if(!string.IsNullOrEmpty(Password) && Password != ConfirmPassword)
                        return "These passwords do not match. Please try again.";
                } else if(columnName == birthdayProp)
                    return RequiredValidationRule.GetErrorMessage(birthdayProp, Birthday);
                else if(columnName == genderProp)
                    return RequiredValidationRule.GetErrorMessage(genderProp, Gender, -1);
                return null;
            }
        }
    }
}