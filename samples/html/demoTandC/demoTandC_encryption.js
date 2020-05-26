var js = null;

function toHex(value, padding)
{
  var hex = value.toString(16);
  return "0000000000000000".substr(0,padding-hex.length)+hex;
}

function toHex2(value) { return toHex(value,2); }
function toHex4(value) { return toHex(value,4); }
function toHex8(value) { return toHex(value,8); }

function arrayToHex(v)
{
  var s="";
  for (var i = 0; i < v.length; ++i)
    s = s + toHex2(v[i]);
  return s;
}

function hexToArray(s)
{
  var a = new Array();
  for (var i = 0; i < s.length;i+=2)
    a.push(parseInt("0x"+ s.substr(i,2),16));
  return a;
}

function padLeft(str, len, pad)
{
  if (typeof(pad) == "undefined") pad = ' ';
  str = str.toString();
  if (len > str.length)
    str = Array(len+1-str.length).join(pad) + str;
  return str;
}

function createObject(objectName) {
  var object;
  if (1) {
    object = new ActiveXObject(objectName);
  } else {
    if (!wgssSTU) {
      wgssSTU = document.getElementById("wgssSTU");
    }
    object = wgssSTU.createObject(objectName);

  }

  return object;
}


function encryptionHandler()
{
  if (js == null)
    js = createObject("WacomGSS.STU.JScript");


  var encryptionHandler = new Object();


  encryptionHandler.bigint_p    = null;
  encryptionHandler.bigint_g    = null;
  encryptionHandler.sjcl_keyAES = null;


  encryptionHandler.reset = function() 
  {
    this.bigint_p    = null;
    this.bigint_g    = null;
    this.sjcl_keyAES = null;
  }


  encryptionHandler.clearKeys = function()
  {
    this.sjcl_keyAES = null;
  }


  encryptionHandler.requireDH = function() 
  {
    return this.bigint_p == null || this.bigint_g == null; 
  }


  encryptionHandler.setDH = function(dhPrime, dhBase) 
  { 
    var p = js.toArray(dhPrime);
    var g = js.toArray(dhBase);

    this.bigint_p = str2bigInt(arrayToHex(p), 16, 128);
    this.bigint_g = str2bigInt(arrayToHex(g), 16, 0);
  }


  encryptionHandler.generateHostPublicKey = function() 
  {
    // secret key
    // sample code cheat: hard coded value should be properly generated in a real application.
    this.bigint_a = str2bigInt("F965BC2C949B91938787D5973C94856C", 16, 128);

    // public key
    var bigint_A = powMod(this.bigint_g, this.bigint_a, this.bigint_p);

    var hex_A = padLeft(bigInt2str(bigint_A,16), 32, '0');
    var A = hexToArray(hex_A);
    return js.toVBArray(A);
  }


  encryptionHandler.computeSharedKey = function(devicePublicKey)
  {
    var B = js.toArray(devicePublicKey);
  
    var bigint_B = str2bigInt(arrayToHex(B), 16, 128);

    var bigint_shared = powMod(bigint_B, this.bigint_a, this.bigint_p);

    var str_shared = padLeft(bigInt2str(bigint_shared,16), 32, '0');

    this.sjcl_keyAES = new sjcl.cipher.aes( sjcl.codec.hex.toBits(str_shared) );
  }


  encryptionHandler.decrypt = function(data)
  {
    var arr_cipherText  = js.toArray(data);
    var hex_cipherText  = arrayToHex(arr_cipherText);
    var sjcl_cipherText = sjcl.codec.hex.toBits(hex_cipherText);

    var sjcl_plainText = this.sjcl_keyAES.decrypt(sjcl_cipherText);

    var hex_plainText = sjcl.codec.hex.fromBits(sjcl_plainText);
    var arr_plainText = hexToArray(hex_plainText);
    return js.toVBArray(arr_plainText);
  }


  return encryptionHandler;
}



function encryptionHandler2()
{
  if (js == null)
    js = createObject("WacomGSS.STU.JScript");


  var encryptionHandler2 = new Object();


  encryptionHandler2.bigint_e    = str2bigInt("65537",10,0);
  encryptionHandler2.bigint_d    = null;
  encryptionHandler2.bigint_N    = null;
  encryptionHandler2.sjcl_keyAES = null;


  encryptionHandler2.reset = function() 
  {
    this.bigint_d    = null;
    this.bigint_N    = null;
    this.sjcl_keyAES = null;
  }


  encryptionHandler2.clearKeys = function() 
  {
    this.sjcl_keyAES = null;
  }


  encryptionHandler2.getSymmetricKeyType = function() 
  {
    return 2; // SymmetricKeyType_AES256
  }


  encryptionHandler2.getAsymmetricPaddingType = function() 
  {
    return 0; // AsymmetricPaddingType_None (not recommended for production!)

    //return 2; // AsymmetricPaddingType_OAEP
  }


  encryptionHandler2.getAsymmetricKeyType = function() 
  {
    return 2; // AsymmetricKeyType_RSA2048
  }


  encryptionHandler2.getPublicExponent = function() 
  {
    var hex_e = padLeft(bigInt2str(this.bigint_e,16), 8, '0');
    var E = hexToArray(hex_e);
    return js.toVBArray(E);
  }


  encryptionHandler2.generatePublicKey = function() 
  { 
    if (this.bigint_N != null)
    {
      var hex_N = padLeft(bigInt2str(this.bigint_N,16), 2048/8/2, '0');
      var N = hexToArray(hex_N);
      return js.toVBArray(N);
    }


    var calculatePrimes = true;


    if (calculatePrimes)
    {
      var b = 1024;

      var bigint_p;
      while (1)
      {
        bigint_p = randProbPrime(b);
        //bigint_p = randTruePrime(b);

        if (!equalsInt(mod(bigint_p,this.bigint_e),1))  //the prime must not be congruent to 1 modulo e
          break;
      }

      while(1)
      {
        var bigint_q;
        while (1)
        {
          bigint_q = randProbPrime(b);
          //bigint_q = randTruePrime(b);

          if (bigint_p != bigint_q)
          {
            if (!equalsInt(mod(bigint_q,this.bigint_e),1))  //the prime must not be congruent to 1 modulo e
              break;
          }
        }

        this.bigint_N = mult(bigint_p,bigint_q);


        var bigint_phi = mult( addInt(bigint_p,-1), addInt(bigint_q,-1) )

        this.bigint_d = inverseMod(this.bigint_e,bigint_phi);

        if (this.bigint_d) 
        {
          break;
        }

      }
    }
    else
    {
      // sample code cheat: hard coded value should be properly generated in a real application.
      this.bigint_d = str2bigInt("2B1DD41FDCE1180A098EAFEFD63B8990B3964044BC2F63CB6067FBEFD6E4C76C9399E45E63B01171E9EE920A40753EB37CCBAEDE04BE726C5308FAC39E84D376D618BBC5EF1206A8CA537646DF788BC07163CB851A205DC57B61EE78F52258EDEF65F7371ABF2B10E8BF7930B655184D5EC51B972A3A0D3F5D2009EB0A6B5DFCD8DDD29CA704CDFF2086A211CFE7E0C395E9B53D5B1FF370BFC90C3A8255A64A8674E8FB41002838ABFC430EA558DECFFE1B563D96D06DCAEA8A5793DCA68C3FB4265BCE38CBEFBBAEB3B8FC1689F7B8510BF20B9D72E490887FB36F4722FEB813E6252DDC3BB17DA645ACEE8292AB85FA1A3048B7BBB34F3B50489BE7913421",16);
      this.bigint_N = str2bigInt("93DDCD8BC9E478491C54413F0484FE79DDDA464A0F53AC043C6194FD473FB75B893C783F56701D2D30B021C4EE0401F058B98F035804CFBB0E67A8136A2F052A98037457460FAB7B3B148EC7C95604FF2192EA03FCC04285EC539DDF3375678E4C4D926163ABBC609C41EF5673C449DF5AC74FFA8150D33FC5436C5CC2621E642C42C10E71BF3895B07A52E7D86C84D3A9269462CF2E484E17D34DEDFF9090D6745A00EF40EE33C71C5688E856AF3C6C42AF3C4C8523711498F4508DC18BC5E24F38C2C7E971BA61BB24B19E3AE74D4D57023AF59BA9D979FCF48080E18D920E31A319C544DEA0E9DAF088E09B6098C07C20328DD0F62C5C99FCD2EB7C4F7CD3",16);
    }

    var hex_N = padLeft(bigInt2str(this.bigint_N,16), 2048/8/2, '0');
    var N = hexToArray(hex_N);

    return js.toVBArray(N);
  }


  encryptionHandler2.computeSessionKey = function(data) 
  { 
    var arr_data = js.toArray(data);
  
    var bigint_c = str2bigInt(arrayToHex(arr_data), 16, 2048);

    var bigint_m = powMod(bigint_c, this.bigint_d, this.bigint_N);

    var hex_k = padLeft(bigInt2str(bigint_m,16), 256/8*2, '0');

    this.sjcl_keyAES = new sjcl.cipher.aes( sjcl.codec.hex.toBits(hex_k) );
  }


  encryptionHandler2.decrypt = function(data)
  {
    var arr_cipherText  = js.toArray(data);
    var hex_cipherText  = arrayToHex(arr_cipherText);
    var sjcl_cipherText = sjcl.codec.hex.toBits(hex_cipherText);

    var sjcl_plainText = this.sjcl_keyAES.decrypt(sjcl_cipherText);

    var hex_plainText = sjcl.codec.hex.fromBits(sjcl_plainText);
    var arr_plainText = hexToArray(hex_plainText);

    return js.toVBArray(arr_plainText);
  }


  return encryptionHandler2;
}
