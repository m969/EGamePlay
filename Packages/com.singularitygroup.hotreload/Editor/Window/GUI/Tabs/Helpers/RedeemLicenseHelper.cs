using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal enum RedeemStage {
        None,
        Registration,
        Redeem,
        Login
    }

    // IMPORTANT: don't rename
    internal enum RegistrationOutcome {
        None,
        Indie,
        Business,
    }

    internal class RedeemLicenseHelper {
        public static readonly RedeemLicenseHelper I = new RedeemLicenseHelper();

        private string _pendingCompanySize;
        private string _pendingInvoiceNumber;
        private string _pendingRedeemEmail;

        private const string registerFlagPath = PackageConst.LibraryCachePath + "/registerFlag.txt";
        public const string registerOutcomePath = PackageConst.LibraryCachePath + "/registerOutcome.txt";

        public RedeemStage RedeemStage { get; private set; }
        public RegistrationOutcome RegistrationOutcome { get; private set; }
        public bool RegistrationRequired => RedeemStage != RedeemStage.None;

        private string status;
        private string error;

        const string statusSuccess = "success";
        const string statusAlreadyClaimed = "already redeemed by this user/device";
        const string unknownError = "We apologize, an error happened while redeeming your license. Please reach out to customer support for assistance.";

        private GUILayoutOption[] secondaryButtonLayoutOptions = new[] { GUILayout.MaxWidth(100) };

        private bool requestingRedeem;
        private HttpClient redeemClient;
        const string redeemUrl = "https://vmhzj6jonn3qy7hk7tx7levpli0bstpj.lambda-url.us-east-1.on.aws/redeem";

        public RedeemLicenseHelper() {
            if (File.Exists(registerFlagPath)) {
                RedeemStage = RedeemStage.Registration;
            }
            try {
                if (File.Exists(registerOutcomePath)) {
                    RegistrationOutcome outcome;
                    if (Enum.TryParse(File.ReadAllText(registerOutcomePath), out outcome)) {
                        RegistrationOutcome = outcome;
                    }
                }
            } catch (Exception e) {
                Log.Warning($"Failed determining registration outcome with {e.GetType().Name}: {e.Message}");
            }
        }

        public void RenderStage(HotReloadRunTabState state) {
            if (state.redeemStage == RedeemStage.Registration) {
                RenderRegistration();
            } else if (state.redeemStage == RedeemStage.Redeem) {
                RenderRedeem();
            } else if (state.redeemStage == RedeemStage.Login) {
                RenderLogin(state);
            }
        }

        private void RenderRegistration() {
            var message = PackageConst.IsAssetStoreBuild
                ? "Unity Pro users are required to obtain an additional license. You are eligible to redeem one if your company has ten or fewer employees. Please enter your company details below."
                : "The licensing model for Unity Pro users varies depending on the number of employees in your company. Please enter your company details below.";
            if (error != null) {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            } else {
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Company size (number of employees)");
            GUI.SetNextControlName("company_size");
            _pendingCompanySize = EditorGUILayout.TextField(_pendingCompanySize)?.Trim();
            EditorGUILayout.Space();

            if (GUILayout.Button("Proceed")) {
                int companySize;
                if (!int.TryParse(_pendingCompanySize, out companySize)) {
                    error = "Please enter a number.";
                } else {
                    error = null;
                    HandleRegistration(companySize);
                }
            }
        }

        void HandleRegistration(int companySize) {
            RequestHelper.RequestEditorEvent(new Stat(StatSource.Client, StatLevel.Debug, StatFeature.Licensing, StatEventType.Register), new EditorExtraData { { StatKey.CompanySize, companySize } });
            if (companySize > 10) {
                FinishRegistration(RegistrationOutcome.Business);
                EditorCodePatcher.DownloadAndRun().Forget();
            } else if (PackageConst.IsAssetStoreBuild) {
                SwitchToStage(RedeemStage.Redeem);
            } else {
                FinishRegistration(RegistrationOutcome.Indie);
                EditorCodePatcher.DownloadAndRun().Forget();
            }
        }

        private void RenderRedeem() {
            if (error != null) {
                EditorGUILayout.HelpBox(error, MessageType.Warning);
            } else {
                EditorGUILayout.HelpBox("To enable us to verify your purchase, please enter your invoice number/order ID. Additionally, provide the email address that you intend to use for managing your credentials.", MessageType.Info);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Invoice number/Order ID");
            GUI.SetNextControlName("invoice_number");
            _pendingInvoiceNumber = EditorGUILayout.TextField(_pendingInvoiceNumber ?? HotReloadPrefs.RedeemLicenseInvoice)?.Trim();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Email");
            GUI.SetNextControlName("email_redeem");
            _pendingRedeemEmail = EditorGUILayout.TextField(_pendingRedeemEmail ?? HotReloadPrefs.RedeemLicenseEmail);
            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(requestingRedeem)) {
                if (GUILayout.Button("Redeem", HotReloadRunTab.bigButtonHeight)) {
                    RedeemLicense(email: _pendingRedeemEmail, invoiceNumber: _pendingInvoiceNumber).Forget();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Skip", secondaryButtonLayoutOptions)) {
                    SwitchToStage(RedeemStage.Login);
                }
                GUILayout.FlexibleSpace();
            }
        }

        async Task RedeemLicense(string email, string invoiceNumber) {
            string validationError;
            if (string.IsNullOrEmpty(invoiceNumber)) {
                validationError = "Please enter invoice number / order ID.";
            } else {
                validationError = HotReloadRunTab.ValidateEmail(email);
            }
            if (validationError != null) {
                error = validationError;
                return;
            }
            var resp = await RequestRedeem(email: email, invoiceNumber: invoiceNumber);
            status = resp?.status;
            if (status != null) {
                if (status != statusSuccess && status != statusAlreadyClaimed) {
                    Log.Error("Redeeming license failed: unknown status received");
                    error = unknownError;
                } else {
                    HotReloadPrefs.RedeemLicenseEmail = email;
                    HotReloadPrefs.RedeemLicenseInvoice = invoiceNumber;
                    // prepare data for login screen
                    HotReloadPrefs.LicenseEmail = email;
                    HotReloadPrefs.LicensePassword = null;

                    SwitchToStage(RedeemStage.Login);
                }
            } else if (resp?.error != null) {
                Log.Warning($"Redeeming a license failed with error: {resp.error}");
                error = GetPrettyError(resp);
            } else {
                Log.Warning("Redeeming a license failed: uknown error encountered");
                error = unknownError;
            }
        }

        string GetPrettyError(RedeemResponse response) {
            var err = response?.error;
            if (err == null) {
                return unknownError;
            }
            if (err.Contains("Invalid email")) {
                return "Please enter a valid email address.";
            } else if (err.Contains("License invoice already redeemed")) {
                return "The invoice number/order ID you're trying to use has already been applied to redeem a license. Please enter a different invoice number/order ID. If you have already redeemed a license for another email, you may proceed to the next step.";
            } else if (err.Contains("Different license already redeemed by given email")) {
                return "The provided email has already been used to redeem a license. If you have previously redeemed a license, you can proceed to the next step and use your existing credentials. If not, please input a different email address.";
            } else if (err.Contains("Invoice not found")) {
                return "The invoice was not found. Please ensure that you've entered the correct invoice number/order ID.";
            } else if (err.Contains("Invoice refunded")) {
                return "The purchase has been refunded. Please enter a different invoice number/order ID.";
            } else {
                return unknownError;
            }
        }

        async Task<RedeemResponse> RequestRedeem(string email, string invoiceNumber) {
            requestingRedeem = true;
            await ThreadUtility.SwitchToThreadPool();
            try {
                redeemClient = redeemClient ?? (redeemClient = HttpClientUtils.CreateHttpClient());
                var input = new Dictionary<string, string> {
                    { "email", email },
                    { "invoice", invoiceNumber }
                };
                var content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
                using (var resp = await redeemClient.PostAsync(redeemUrl, content, HotReloadWindow.Current.cancelToken).ConfigureAwait(false)) {
                    if (resp.StatusCode != HttpStatusCode.OK) {
                        return new RedeemResponse(null, $"Redeem request failed. Status code: {(int)resp.StatusCode}, reason: {resp.ReasonPhrase}");
                    }
                    var str = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try {
                        return JsonConvert.DeserializeObject<RedeemResponse>(str);
                    } catch (Exception ex) {
                        return new RedeemResponse(null, $"Failed deserializing redeem response with exception: {ex.GetType().Name}: {ex.Message}");
                    }
                }
            } catch (WebException ex) {
                return new RedeemResponse(null, $"Redeeming license failed: WebException encountered {ex.Message}");
            } finally {
                requestingRedeem = false;
            }
        }

        private class RedeemResponse {
            public string status;
            public string error;

            public RedeemResponse(string status, string error) {
                this.status = status;
                this.error = error;
            }
        }

        private void RenderLogin(HotReloadRunTabState state) {
            if (status == statusSuccess) {
                EditorGUILayout.HelpBox("Success! You will receive an email containing your license password shortly. Once you receive it, please enter the received password in the designated field below to complete your registration.", MessageType.Info);
            } else if (status == statusAlreadyClaimed) {
                EditorGUILayout.HelpBox("Your license has already been redeemed. Please enter your existing password below.", MessageType.Info);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            HotReloadRunTab.RenderLicenseInnerPanel(state, renderLogout: false);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Go Back", secondaryButtonLayoutOptions)) {
                    SwitchToStage(RedeemStage.Redeem);
                }
                GUILayout.FlexibleSpace();
            }
        }

        public void StartRegistration() {
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(registerFlagPath));
            using (File.Create(registerFlagPath)) {
            }
            RedeemStage = RedeemStage.Registration;
            RegistrationOutcome = RegistrationOutcome.None;
        }

        public void FinishRegistration(RegistrationOutcome outcome) {
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(registerFlagPath));
            File.WriteAllText(registerOutcomePath, outcome.ToString());
            File.Delete(registerFlagPath);
            RegistrationOutcome = outcome;
            SwitchToStage(RedeemStage.None);
            Cleanup();
        }

        void SwitchToStage(RedeemStage stage) {
            // remove focus so that the input field re-renders
            GUI.FocusControl(null);
            RedeemStage = stage;
        }

        void Cleanup() {
            redeemClient?.Dispose();
            redeemClient = null;
            _pendingCompanySize = null;
            _pendingInvoiceNumber = null;
            _pendingRedeemEmail = null;
            status = null;
            error = null;
        }
    }
}