using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Text;
using System.Security.Cryptography;

public class DatabaseController {

    string EncryptionKey = "abcdefghijklmnop";
    string EncryptionIv = "ponmlkjihgfedcba";

    public SQLiteConnection connection
    {
        get
        { 
            return _connection;
        }
    }

    private SQLiteConnection _connectionVersionCheck;

    private SQLiteConnection _connection;

    GameObject go;

    public DatabaseController(string DatabaseName)
    {

        go = GameObject.Find("StartupText");
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
        _connectionVersionCheck = new SQLiteConnection(string.Format(@"Assets/StreamingAssets/{0}", DatabaseName), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);
#endif

            Debug.Log("Database written");
        }
        else{
#if UNITY_ANDROID
            var tempFilepath = "";
            //try { 
                var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                tempFilepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName.Replace(".", "2."));
                File.WriteAllBytes(tempFilepath, loadDb.bytes);
                _connectionVersionCheck = new SQLiteConnection(tempFilepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                go.GetComponent<Text>().text += "Created new Connection check database!\n";
            //}
            //catch (Exception e)
            //{
            //    go.GetComponent<Text>().text += e.Message + "\n Path DB.db: " + filepath + "\n Path DB2.db: " + tempFilepath;
            //}
            //try
            //{
                DecryptDB(filepath);
            //}
            //catch(Exception e)
            //{
            //    go.GetComponent<Text>().text += "Decription error: " + e.Message + "\n" + filepath + "\n";
            //}
#endif
        }
        var dbPath = filepath;
#endif


        //_connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);
        CheckIfDBUpToDate();
    }

    public void CheckIfDBUpToDate() { 
        if (_connectionVersionCheck == null)
        {
            go.GetComponent<Text>().text += "New database so it's up to date";
            return;
        }
        int versionId = -1;
        try
        {
            versionId = _connection.Table<Version>().First().versionId;
            Debug.Log("Current Version ID: " + versionId);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        int newDbId = -1; 
        try
        {
            newDbId = _connectionVersionCheck.Table<Version>().First().versionId;
            Debug.Log("New Version ID: " + newDbId);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message + " \n " + _connectionVersionCheck + "\n It throws an error here?");
        }
        //als versie verouderd is
        if (newDbId > versionId)
        {
            Debug.Log("New version");
            
            if (go != null)
                Debug.Log("\nThere is a new version");

            UpdateDB();
        }
        else
        {
            if (go != null)
                Debug.Log("Same Version");
#if !UNITY_EDITOR
            File.Delete(_connectionVersionCheck.DatabasePath);
#endif
        }
    }

    public void DecryptDB(string dbPath)
    {

        byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
        byte[] iv = Encoding.UTF8.GetBytes(EncryptionIv);

        var encMessage = File.ReadAllBytes(dbPath);

        byte[] decMessage;
        using (var rijndael = new RijndaelManaged())
        {
            rijndael.Key = key;
            rijndael.IV = iv;
            decMessage = DecryptBytes(rijndael, encMessage);
        }

        File.WriteAllBytes(dbPath, decMessage);
    }

    public bool EncryptDB()
    {
        _connection.Close();
        string dbPath = _connection.DatabasePath;
        _connection = null;
        var file = File.ReadAllBytes(dbPath);
        //var encryptedFile = EncryptorDecryptor.EncryptDecrypt(file);
        //File.Delete(dbPath);
        byte[] encMessage;
        byte[] key;
        byte[] iv;

        key = Encoding.UTF8.GetBytes(EncryptionKey);
        iv = Encoding.UTF8.GetBytes(EncryptionIv);
        using (var rijndael = new RijndaelManaged())
        {
            rijndael.Key = key;
            rijndael.IV = iv;
            encMessage = EncryptBytes(rijndael, file);
        }
        Debug.Log("\nEncrypt: " + dbPath);
        File.WriteAllBytes(dbPath, encMessage);
        return true;
    }

    private static byte[] EncryptBytes(
        SymmetricAlgorithm alg,
        byte[] message)
    {
        if ((message == null) || (message.Length == 0))
        {
            return message;
        }

        if (alg == null)
        {
            throw new ArgumentNullException("alg");
        }

        using (var stream = new MemoryStream())
        using (var encryptor = alg.CreateEncryptor())
        using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
        {
            encrypt.Write(message, 0, message.Length);
            encrypt.FlushFinalBlock();
            return stream.ToArray();
        }
    }

    private static byte[] DecryptBytes(
        SymmetricAlgorithm alg,
        byte[] message)
    {
        if ((message == null) || (message.Length == 0))
        {
            return message;
        }

        if (alg == null)
        {
            throw new ArgumentNullException("alg");
        }

        using (var stream = new MemoryStream())
        using (var decryptor = alg.CreateDecryptor())
        using (var encrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
        {
            encrypt.Write(message, 0, message.Length);
            encrypt.FlushFinalBlock();
            return stream.ToArray();
        }
    }

    public void UpdateDB()
    {
        /* Update
         * 
         * Boats
         * Items
         * Locations
         * Opponent setups
         * Persons
         * Races
         * Routes
         * Sails
         * Track
         * Upgrade
         * Achievements
         * AchievementProperty
         * Version
        */
        //Boats
        foreach (Boat b in _connectionVersionCheck.Table<Boat>().ToList())
        {
            TableQuery<Boat> temp = _connection.Table<Boat>().Where(x => x.id == b.id);
            if (temp.Count() > 0)
            {
                    Boat oldb = temp.First();
                    oldb.Sails = b.Sails;
                    oldb.Upgrades = b.Upgrades;
                    oldb.Price = b.Price;
                    oldb.OfflineSpeed = b.OfflineSpeed;
                    oldb.NGZLimit = b.NGZLimit;
                    oldb.Name = b.Name;
                    _connection.Update(oldb);
            }
            else
            {
                _connection.Insert(b);
            }
        }
        //Items
        try { 
        foreach (Item i in _connectionVersionCheck.Table<Item>().ToList())
        {
            TableQuery<Item> temp = _connection.Table<Item>().Where(x => x.id == i.id);
            if (temp.Count() > 0)
            {
                Item oldi = temp.First();
                oldi.Name = i.Name;
                oldi.Price = i.Price;
                oldi.Description = i.Description;
                    _connection.Update(oldi);
            }
            else
            {
                    _connection.Insert(i);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Items";
        }
        //Locations
        try { 
        foreach (Location l in _connectionVersionCheck.Table<Location>().ToList())
        {
            TableQuery<Location> temp = _connection.Table<Location>().Where(x => x.id == l.id);
            if(temp.Count() > 0)
            {
                Location oldl = temp.First();
                oldl.AvaliableBoats = l.AvaliableBoats;
                    _connection.Update(oldl);
            }
            else
            {
                    _connection.Insert(l);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Locations";
        }
        //Opponent Setups
        try { 
        foreach (OpponentSetup o in _connectionVersionCheck.Table<OpponentSetup>().ToList())
        {
            TableQuery<OpponentSetup> temp = _connection.Table<OpponentSetup>().Where(x => x.id == o.id);
            if(temp.Count() > 0)
            {
                OpponentSetup oldo = temp.First();
                oldo.TackingAccuracy = o.TackingAccuracy;
                oldo.TackingExtra = o.TackingExtra;
                oldo.TackingSpeed = o.TackingSpeed;
                oldo.BestUpwindAngle = o.BestUpwindAngle;
               _connection.Update(oldo);
            }
            else
            {
                    _connection.Insert(o);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "OpponentSetups";
        }
        //Persons
        try { 
        foreach (Person p in _connectionVersionCheck.Table<Person>().ToList())
        {
            TableQuery<Person> temp = _connection.Table<Person>().Where(x => x.id == p.id);
            if(temp.Count() > 0)
            {
                Person oldp = temp.First();
                oldp.Name = p.Name;
                oldp.BoatId = p.BoatId;
                oldp.LocationId = p.LocationId;
                oldp.OpponentSetupId = p.OpponentSetupId;
                    _connection.Update(oldp);
            }
            else
            {
                    _connection.Insert(p);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Persons";
        }
        //Races
        try { 
        foreach (Race r in _connectionVersionCheck.Table<Race>().ToList())
        {
            TableQuery<Race> temp = _connection.Table<Race>().Where(x => x.id == r.id);
            if(temp.Count() > 0)
            {
                Race oldr = temp.First();
                oldr.Name = r.Name;
                oldr.BoatsUsedId = r.BoatsUsedId;
                oldr.Difficulty = r.Difficulty;
                oldr.LocationId = r.LocationId;
                oldr.TrackId = r.TrackId;
                    _connection.Update(oldr);
            }
            else
            {
                    _connection.Insert(r);
                }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Races";
        }
        //Routes
        try { 
        foreach (Route r in _connectionVersionCheck.Table<Route>().ToList())
        {
            TableQuery<Route> temp = _connection.Table<Route>().Where(x => x.id == r.id);
            if(temp.Count() > 0)
            {
                Route oldr = temp.First();
                oldr.route = r.route;
                    _connection.Update(oldr);
            }
            else
            {
                    _connection.Insert(r);
                }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Routes";
        }
        //Sails
        try { 
        foreach (Sail s in _connectionVersionCheck.Table<Sail>().ToList())
        {
            TableQuery<Sail> temp = _connection.Table<Sail>().Where(x => x.id == s.id);
            if(temp.Count() > 0)
            {
                Sail olds = temp.First();
                olds.Name = s.Name;
                olds.Price = s.Price;
                olds.MaxWindSpeed = s.MaxWindSpeed;
                olds.DamageModifier = s.DamageModifier;
                olds.SpeedModifier = s.SpeedModifier;
                olds.OfflineSpeedModifier = s.OfflineSpeedModifier;
                    _connection.Update(olds);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Sails";
        }
        //Track
        try { 
        foreach (Track t in _connectionVersionCheck.Table<Track>().ToList())
        {
            TableQuery<Track> temp = _connection.Table<Track>().Where(x => x.id == t.id);
            if(temp.Count() > 0)
            {
                Track oldt = temp.First();
                oldt.Name = t.Name;
                oldt.Waypoints = t.Waypoints;
                oldt.WaypointOrder = t.WaypointOrder;
                    _connection.Update(oldt);
            }
            else
            {
                _connection.Insert(t);
            }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Tracks";
        }
        //Upgrade
        try { 
        foreach (Upgrade u in _connectionVersionCheck.Table<Upgrade>().ToList())
        {
            TableQuery<Upgrade> temp = _connection.Table<Upgrade>().Where(x => x.id == u.id);
            if(temp.Count() > 0)
            {
                Upgrade oldu = temp.First();
                oldu.Name = u.Name;
                oldu.Price = u.Price;
                oldu.HeightModifier = u.HeightModifier;
                oldu.SpeedModifier = u.SpeedModifier;
                oldu.DamageModifier = u.DamageModifier;
                oldu.OfflineSpeedModifier = u.OfflineSpeedModifier;
                    _connection.Update(oldu);
            }
            else
            {
                    _connection.Insert(u);
                }
        }
        }
        catch
        {
            go.GetComponent<Text>().text += "Upgrades";
        }

        if(_connection.Table<Achievement>() == null)
        {
            _connection.CreateTable(typeof(Achievement));
        }

        //Achivements
        foreach(Achievement a in _connectionVersionCheck.Table<Achievement>())
        {
            TableQuery<Achievement> temp = _connection.Table<Achievement>().Where(x => x.id == a.id);
            if(temp.Count() > 0)
            {
                Achievement olda = temp.First();
                olda.Name = a.Name;
                olda.Description = a.Description;
                olda.Property = a.Property;
                olda.PropertyAmount = a.PropertyAmount;
                _connection.Update(olda);
            }
            else
            {
                _connection.Insert(a);
            }
        }

        if(_connection.Table<AchievementProperty>() == null)
        {
            _connection.CreateTable(typeof(AchievementProperty));
        }
        //AchievementProperty
        foreach(AchievementProperty a in _connectionVersionCheck.Table<AchievementProperty>())
        {
            TableQuery<AchievementProperty> temp = _connection.Table<AchievementProperty>().Where(x => x.id == a.id);
            if(temp.Count() > 0)
            {
                AchievementProperty olda = temp.First();
                olda.Name = a.Name;
                _connection.Update(olda);
            }
            else
            {
                _connection.Insert(a);
            }
        }

        //Version
        try { 
        Version v = _connectionVersionCheck.Table<Version>().First();
        TableQuery<Version> tempV = _connection.Table<Version>();
        if(tempV.Count() > 0)
        {
            Version oldv = tempV.First();
            oldv.versionId = v.versionId;
                _connection.Update(oldv);
        }
        else
        {
                _connection.Insert(v);
            }
        }
        catch(Exception e)
        {
            go.GetComponent<Text>().text += "\nVersion\n" + e.Message;
        }
    }
}

public static class EncryptorDecryptor
{
    public static int key = 129;

    public static string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }
}
