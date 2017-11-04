using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness.Request;
using Android.Gms.Common;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Result;
using Java.Util.Concurrent;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Util;
using System.Collections.Generic;

namespace APIFitnessApp.Droid
{ 
    [Activity(Label = "APIFitnessApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public partial class MainActivity : AppCompatActivity
    {
        const int REQUEST_OAUTH = 1;
        const string AUTH_PENDING = "auth_state_pending";
        private bool authInProgress = false;
        private GoogleApiClient mClient;
        public const string TAG = "APIFitnessApp";

        /* 
         * En este metodo se crea la conexion a la API fitness
         * iniciando GoogleApiClient
         */
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            if (savedInstanceState != null){
                authInProgress = savedInstanceState.GetBoolean(AUTH_PENDING);
            }
            Console.WriteLine("Creando el objeto API Fitness");
            //Creamos la instancia de la API fitness
            BuildFitnessClient();
        }

        // Intenta conectar al iniciar la actividad
        protected override void OnStart()
        {
            base.OnStart();
            Log.Info(TAG, "Connecting...");
            Console.WriteLine("Connecting...");
            mClient.Connect();
        }

        // Cierra la conexion cuando acaba la actividad
        protected override void OnStop()
        {
            if (mClient.IsConnected)
            {
                Console.WriteLine("Desconectando..");
                mClient.Disconnect();
            }
            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            if (Log.IsLoggable(TAG, LogPriority.Info)){
                Log.Info(TAG, "In onActivityResult");
            }
            if (requestCode == REQUEST_OAUTH){
                authInProgress = false;
                if (resultCode == Android.App.Result.Ok){
                    // Aseguramos que la app no esta intentando conectarse o esta conectada
                    if (!mClient.IsConnecting && !mClient.IsConnected){
                        mClient.Connect();
                    }
                }
            }

        }
        //Almacena el acceso a la API para el usuario
        protected override void OnSaveInstanceState(Bundle bundle)
        {
            bundle.PutBoolean("authInProgress", authInProgress);
            base.OnSaveInstanceState(bundle);
        }

        /*
         * Metodo que genera la llamada a la API y la gestion de eventos
         */
        private void BuildFitnessClient()
        {
            // Create and connect the Google API client
            mClient = new GoogleApiClient.Builder(this)
                .AddApi(FitnessClass.HISTORY_API)
                .AddScope(new Scope(Scopes.FitnessLocationRead))
                .AddConnectionCallbacks(
                    // connection succeeded
                    connectionHint => {
                        if (Log.IsLoggable(TAG, LogPriority.Info))
                        {
                            Log.Info(TAG, "Connected to the Google API client");
                        }
                        // Get step data from Google Play Services
                        readSteps();
                    },
                    // connection suspended
                    cause => {
                        if (Log.IsLoggable(TAG, LogPriority.Info))
                        {
                            Log.Info(TAG, "Connection suspended");
                        }
                    }
                )
                .AddOnConnectionFailedListener(
                    // connection failed
                    result => {
                        if (Log.IsLoggable(TAG, LogPriority.Info)){
                            Log.Info(TAG, "Failed to connect to the Google API client");
                        }
                        if (!result.HasResolution){
                            GoogleApiAvailability.Instance.GetErrorDialog(this, result.ErrorCode, 0).Show();
                            return;
                        }

                        if (!authInProgress){
                            try{
                                if (Log.IsLoggable(TAG, LogPriority.Info)){
                                    Log.Info(TAG, "Attempting to resolve failed connection");
                                }
                                authInProgress = true;
                                result.StartResolutionForResult(this, REQUEST_OAUTH);
                            }catch (IntentSender.SendIntentException e){
                                if (Log.IsLoggable(TAG, LogPriority.Error)){
                                    Log.Error(TAG, "Exception while starting resolution activity", e);
                                    authInProgress = false;
                                }
                            }
                        }
                    }
                )
                .Build();
        }
    }

    /*
	 * Implements the Google Fit data access
	 */
    public partial class MainActivity : IResultCallback
    {
        private void readSteps()
        {
            DataReadRequest readRequest = new DataReadRequest.Builder()
                .Aggregate(Android.Gms.Fitness.Data.DataType.TypeStepCountDelta, Android.Gms.Fitness.Data.DataType.AggregateStepCountDelta)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.TwoDaysAgoMillis(), TimeUtility.CurrentMillis(), TimeUnit.Milliseconds)
                .Build();

            FitnessClass.HistoryApi.ReadData(mClient, readRequest).SetResultCallback(this);
        }

        public void OnResult(Java.Lang.Object obj)
        {
            IList<Bucket> buckets = ((DataReadResult)obj).Buckets;
            // There should be at least two buckets; the last being the latest 24 hour period
            if (buckets.Count < 2)
            {
                if (Log.IsLoggable(TAG, LogPriority.Error))
                {
                    Log.Error(TAG, $"Too few buckets returned: {buckets.Count}");
                }
                return;
            }

            // last bucket is previous 24 hours
            int last24Hours = ExtractStepValue(buckets[buckets.Count - 1]);
            // second-last bucket's the 24 hours previous to the above
            int last48Hours = ExtractStepValue(buckets[buckets.Count - 2]) + last24Hours;

            (FindViewById<TextView>(Resource.Id.past24Value)).Text = last24Hours.ToString();
            (FindViewById<TextView>(Resource.Id.past48Value)).Text = last48Hours.ToString();
        }

        private int ExtractStepValue(Bucket bucket)
        {
            // There should only be 1 data set and 0 or 1 data points, but for
            // clarity we'll loop through to show the structures
            int steps = 0;
            foreach (DataSet ds in bucket.DataSets)
            {
                foreach (DataPoint p in ds.DataPoints)
                {
                    foreach (Field f in p.DataType.Fields)
                    {
                        steps += p.GetValue(f).AsInt();
                    }
                }
            }
            return steps;
        }
    }
}

