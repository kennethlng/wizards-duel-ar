//-----------------------------------------------------------------------
// <copyright file="CloudAnchorUIController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.CloudAnchor
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.CrossPlatform;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controller managing UI for the Cloud Anchor Example.
    /// </summary>
    public class CloudAnchorUIController : MonoBehaviour
    {
        /// <summary>
        /// A gameobject parenting UI for displaying feedback and errors.
        /// </summary>
        public Text snackbarText;

        /// <summary>
        /// A text element displaying the current Room.
        /// </summary>
        public Text roomText;

        /// <summary>
        /// A text element displaying the device's IP Address.
        /// </summary>
        public Text ipAddressText;

        /// <summary>
        /// The host anchor mode button.
        /// </summary>
        public Button hostButton;

        /// <summary>
        /// The resolve anchor mode button.
        /// </summary>
        public Button joinButton;

        /// <summary>
        /// The root for the input interface.
        /// </summary>
        public GameObject joinInputRoot;

        /// <summary>
        /// The input field for the room.
        /// </summary>
        public InputField roomInputField;

        /// <summary>
        /// The input field for the ip address.
        /// </summary>
        public InputField ipAddressInputField;

        /// <summary>
        /// The field for toggling loopback (local) anchor resoltion.
        /// </summary>
        public Toggle ResolveOnDeviceToggle;

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            ipAddressText.text = "My IP Address: " + Network.player.ipAddress;
        }

        /// <summary>
        /// Shows UI for application "Ready Mode".
        /// </summary>
        public void ShowReadyMode()
        {
            hostButton.GetComponentInChildren<Text>().text = "Host";
            hostButton.interactable = true;
            joinButton.GetComponentInChildren<Text>().text = "Resolve";
            joinButton.interactable = true;
            snackbarText.text = "Please select Host or Resolve to continue";
            joinInputRoot.SetActive(false);
        }

        /// <summary>
        /// Shows UI for the beginning phase of application "Hosting Mode".
        /// </summary>
        /// <param name="snackbarText">Optional text to put in the snackbar.</param>
        public void ShowHostingModeBegin(string snackbarText = null)
        {
            hostButton.GetComponentInChildren<Text>().text = "Cancel";
            hostButton.interactable = true;
            joinButton.GetComponentInChildren<Text>().text = "Resolve";
            joinButton.interactable = false;

            if (string.IsNullOrEmpty(snackbarText))
            {
                this.snackbarText.text =
                    "The room code is now available. Please place an anchor to host, press Cancel to Exit.";
            }
            else
            {
                this.snackbarText.text = snackbarText;
            }

            joinInputRoot.SetActive(false);
        }

        /// <summary>
        /// Shows UI for the attempting to host phase of application "Hosting Mode".
        /// </summary>
        public void ShowHostingModeAttemptingHost()
        {
            hostButton.GetComponentInChildren<Text>().text = "Cancel";
            hostButton.interactable = false;
            joinButton.GetComponentInChildren<Text>().text = "Resolve";
            joinButton.interactable = false;
            snackbarText.text = "Attempting to host anchor...";
            joinInputRoot.SetActive(false);
        }

        /// <summary>
        /// Shows UI for the beginning phase of application "Resolving Mode".
        /// </summary>
        /// <param name="snackbarText">Optional text to put in the snackbar.</param>
        public void ShowResolvingModeBegin(string snackbarText = null)
        {
            hostButton.GetComponentInChildren<Text>().text = "Host";
            hostButton.interactable = false;
            joinButton.GetComponentInChildren<Text>().text = "Cancel";
            joinButton.interactable = true;

            if (string.IsNullOrEmpty(snackbarText))
            {
                this.snackbarText.text = "Input Room and IP address to resolve anchor.";
            }
            else
            {
                this.snackbarText.text = snackbarText;
            }

            joinInputRoot.SetActive(true);
        }

        /// <summary>
        /// Shows UI for the attempting to resolve phase of application "Resolving Mode".
        /// </summary>
        public void ShowResolvingModeAttemptingResolve()
        {
            hostButton.GetComponentInChildren<Text>().text = "Host";
            hostButton.interactable = false;
            joinButton.GetComponentInChildren<Text>().text = "Cancel";
            joinButton.interactable = false;
            snackbarText.text = "Attempting to resolve anchor.";
            joinInputRoot.SetActive(false);
        }

        /// <summary>
        /// Shows UI for the successful resolve phase of application "Resolving Mode".
        /// </summary>
        public void ShowResolvingModeSuccess()
        {
            hostButton.GetComponentInChildren<Text>().text = "Host";
            hostButton.interactable = false;
            joinButton.GetComponentInChildren<Text>().text = "Cancel";
            joinButton.interactable = true;
            snackbarText.text = "The anchor was successfully resolved.";
            joinInputRoot.SetActive(false);
        }

        /// <summary>
        /// Sets the room number in the UI.
        /// </summary>
        /// <param name="roomNumber">The room number to set.</param>
        public void SetRoomTextValue(int roomNumber)
        {
            roomText.text = "Room: " + roomNumber;
        }

        /// <summary>
        /// Gets the value of the resolve on device checkbox.
        /// </summary>
        /// <returns>The value of the resolve on device checkbox.</returns>
        public bool GetResolveOnDeviceValue()
        {
            return ResolveOnDeviceToggle.isOn;
        }

        /// <summary>
        /// Gets the value of the room number input field.
        /// </summary>
        /// <returns>The value of the room number input field.</returns>
        public int GetRoomInputValue()
        {
            int roomNumber;
            if (int.TryParse(roomInputField.text, out roomNumber))
            {
                return roomNumber;
            }

            return 0;
        }

        /// <summary>
        /// Gets the value of the ip address input field.
        /// </summary>
        /// <returns>The value of the ip address input field.</returns>
        public string GetIpAddressInputValue()
        {
            return ipAddressInputField.text;
        }

        /// <summary>
        /// Handles a change to the "Resolve on Device" checkbox.
        /// </summary>
        /// <param name="isResolveOnDevice">If set to <c>true</c> resolve on device.</param>
        public void OnResolveOnDeviceValueChanged(bool isResolveOnDevice)
        {
            ipAddressInputField.interactable = !isResolveOnDevice;
        }
    }
}
