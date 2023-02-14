> **Warning**
> 
> As of Umbraco v8.13.0, conditional workflows are now supported natively within Umbraco.
> As such this package is no longer necessary.
> For more info, see [Umbraco.Forms.Issues, issue #370](https://github.com/umbraco/Umbraco.Forms.Issues/issues/370).

# UmbracoForms, Conditional Workflows extension

An extension to Umbraco Forms to allow for emails to be sent conditional upon values set in a form.

## Overview

For example, a dropdown can be created with options for a "Customer Service" department and a "Sales Department". This extension allows an email to be sent to the relevant department conditional upon the value set in this dropdown.

## Installation

Either install from source (copying files over the website root), or as a package from NuGet, with:

`PM> Install-Package UmbracoForms.ConditionalWorkflows`
