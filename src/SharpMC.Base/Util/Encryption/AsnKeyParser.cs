using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpMC.Util.Encryption
{
  class AsnKeyParser
  {
    private AsnParser parser;

    internal AsnKeyParser(String pathname)
    {
      using (var reader = new BinaryReader(
          new FileStream(pathname, FileMode.Open, FileAccess.Read)))
      {
        var info = new FileInfo(pathname);

        parser = new AsnParser(reader.ReadBytes((int)info.Length));
      }
    }

    internal static byte[] TrimLeadingZero(byte[] values)
    {
      byte[] r = null;
      if (0x00 == values[0] && values.Length > 1)
      {
        r = new byte[values.Length - 1];
        Array.Copy(values, 1, r, 0, values.Length - 1);
      }
      else
      {
        r = new byte[values.Length];
        Array.Copy(values, r, values.Length);
      }

      return r;
    }

    internal static bool EqualOid(byte[] first, byte[] second)
    {
      if (first.Length != second.Length)
      { return false; }

      for (var i = 0; i < first.Length; i++)
      {
        if (first[i] != second[i])
        { return false; }
      }

      return true;
    }

    internal RSAParameters ParseRSAPublicKey()
    {
      var parameters = new RSAParameters();

      // Current value
      byte[] value = null;

      // Sanity Check
      var length = 0;

      // Checkpoint
      var position = parser.CurrentPosition();

      // Ignore Sequence - PublicKeyInfo
      length = parser.NextSequence();
      if (length != parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect Sequence RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - AlgorithmIdentifier
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect AlgorithmIdentifier RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();
      // Grab the OID
      value = parser.NextOID();
      byte[] oid = { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
      if (!EqualOid(value, oid))
      { throw new BerDecodeException("Expected OID 1.2.840.113549.1.1.1", position); }

      // Optional Parameters
      if (parser.IsNextNull())
      {
        parser.NextNull();
        // Also OK: value = parser.Next();
      }
      else
      {
        // Gracefully skip the optional data
        value = parser.Next();
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore BitString - PublicKey
      length = parser.NextBitString();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect PublicKey RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - RSAPublicKey
      length = parser.NextSequence();
      if (length < parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect RSAPublicKey RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      parameters.Modulus = TrimLeadingZero(parser.NextInteger());
      parameters.Exponent = TrimLeadingZero(parser.NextInteger());

      Debug.Assert(0 == parser.RemainingBytes());

      return parameters;
    }

    internal RSAParameters ParseRSAPrivateKey()
    {
      var parameters = new RSAParameters();

      // Current value
      byte[] value = null;

      // Checkpoint
      var position = parser.CurrentPosition();

      // Sanity Check
      var length = 0;

      // Ignore Sequence - PrivateKeyInfo
      length = parser.NextSequence();
      if (length != parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect Sequence RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();
      // Version
      value = parser.NextInteger();
      if (0x00 != value[0])
      {
        var sb = new StringBuilder("Incorrect PrivateKeyInfo Version. ");
        var v = new BigInteger(value);
        sb.AppendFormat("Expected: 0, Specified: {0}", v.ToString(10));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - AlgorithmIdentifier
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect AlgorithmIdentifier RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Grab the OID
      value = parser.NextOID();
      byte[] oid = { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
      if (!EqualOid(value, oid))
      { throw new BerDecodeException("Expected OID 1.2.840.113549.1.1.1", position); }

      // Optional Parameters
      if (parser.IsNextNull())
      {
        parser.NextNull();
        // Also OK: value = parser.Next();
      }
      else
      {
        // Gracefully skip the optional data
        value = parser.Next();
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore OctetString - PrivateKey
      length = parser.NextOctetString();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect PrivateKey RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - RSAPrivateKey
      length = parser.NextSequence();
      if (length < parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect RSAPrivateKey RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture),
          parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();
      // Version
      value = parser.NextInteger();
      if (0x00 != value[0])
      {
        var sb = new StringBuilder("Incorrect RSAPrivateKey Version. ");
        var v = new BigInteger(value);
        sb.AppendFormat("Expected: 0, Specified: {0}", v.ToString(10));
        throw new BerDecodeException(sb.ToString(), position);
      }

      parameters.Modulus = TrimLeadingZero(parser.NextInteger());
      parameters.Exponent = TrimLeadingZero(parser.NextInteger());
      parameters.D = TrimLeadingZero(parser.NextInteger());
      parameters.P = TrimLeadingZero(parser.NextInteger());
      parameters.Q = TrimLeadingZero(parser.NextInteger());
      parameters.DP = TrimLeadingZero(parser.NextInteger());
      parameters.DQ = TrimLeadingZero(parser.NextInteger());
      parameters.InverseQ = TrimLeadingZero(parser.NextInteger());

      Debug.Assert(0 == parser.RemainingBytes());

      return parameters;
    }

    internal DSAParameters ParseDSAPublicKey()
    {
      var parameters = new DSAParameters();

      // Current value
      byte[] value = null;

      // Current RenderPosition
      var position = parser.CurrentPosition();
      // Sanity Checks
      var length = 0;

      // Ignore Sequence - PublicKeyInfo
      length = parser.NextSequence();
      if (length != parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect Sequence RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - AlgorithmIdentifier
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect AlgorithmIdentifier RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Grab the OID
      value = parser.NextOID();
      byte[] oid = { 0x2a, 0x86, 0x48, 0xce, 0x38, 0x04, 0x01 };
      if (!EqualOid(value, oid))
      { throw new BerDecodeException("Expected OID 1.2.840.10040.4.1", position); }


      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - DSS-Params
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect DSS-Params RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Next three are curve parameters
      parameters.P = TrimLeadingZero(parser.NextInteger());
      parameters.Q = TrimLeadingZero(parser.NextInteger());
      parameters.G = TrimLeadingZero(parser.NextInteger());

      // Ignore BitString - PrivateKey
      parser.NextBitString();

      // Public Key
      parameters.Y = TrimLeadingZero(parser.NextInteger());

      Debug.Assert(0 == parser.RemainingBytes());

      return parameters;
    }

    internal DSAParameters ParseDSAPrivateKey()
    {
      var parameters = new DSAParameters();

      // Current value
      byte[] value = null;

      // Current RenderPosition
      var position = parser.CurrentPosition();
      // Sanity Checks
      var length = 0;

      // Ignore Sequence - PrivateKeyInfo
      length = parser.NextSequence();
      if (length != parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect Sequence RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();
      // Version
      value = parser.NextInteger();
      if (0x00 != value[0])
      {
        throw new BerDecodeException("Incorrect PrivateKeyInfo Version", position);
      }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - AlgorithmIdentifier
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect AlgorithmIdentifier RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Checkpoint
      position = parser.CurrentPosition();
      // Grab the OID
      value = parser.NextOID();
      byte[] oid = { 0x2a, 0x86, 0x48, 0xce, 0x38, 0x04, 0x01 };
      if (!EqualOid(value, oid))
      { throw new BerDecodeException("Expected OID 1.2.840.10040.4.1", position); }

      // Checkpoint
      position = parser.CurrentPosition();

      // Ignore Sequence - DSS-Params
      length = parser.NextSequence();
      if (length > parser.RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect DSS-Params RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      // Next three are curve parameters
      parameters.P = TrimLeadingZero(parser.NextInteger());
      parameters.Q = TrimLeadingZero(parser.NextInteger());
      parameters.G = TrimLeadingZero(parser.NextInteger());

      // Ignore OctetString - PrivateKey
      parser.NextOctetString();

      // Private Key
      parameters.X = TrimLeadingZero(parser.NextInteger());

      Debug.Assert(0 == parser.RemainingBytes());

      return parameters;
    }
  }

  class AsnParser
  {
    private List<byte> octets;
    private int initialCount;

    internal AsnParser(byte[] values)
    {
      octets = new List<byte>(values.Length);
      octets.AddRange(values);

      initialCount = octets.Count;
    }

    internal int CurrentPosition()
    {
      return initialCount - octets.Count;
    }

    internal int RemainingBytes()
    {
      return octets.Count;
    }

    private int GetLength()
    {
      var length = 0;

      // Checkpoint
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();

        if (b == (b & 0x7f)) { return b; }
        var i = b & 0x7f;

        if (i > 4)
        {
          var sb = new StringBuilder("Invalid Length Encoding. ");
          sb.AppendFormat("Length uses {0} octets",
            i.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        while (0 != i--)
        {
          // shift left
          length <<= 8;

          length |= GetNextOctet();
        }
      }
      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }

      return length;
    }

    internal byte[] Next()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();

        var length = GetLength();
        if (length > RemainingBytes())
        {
          var sb = new StringBuilder("Incorrect RenderSize. ");
          sb.AppendFormat("Specified: {0}, Remaining: {1}",
            length.ToString(CultureInfo.InvariantCulture),
            RemainingBytes().ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        return GetOctets(length);
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal byte GetNextOctet()
    {
      var position = CurrentPosition();

      if (0 == RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          1.ToString(CultureInfo.InvariantCulture),
          RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      var b = GetOctets(1)[0];

      return b;
    }

    internal byte[] GetOctets(int octetCount)
    {
      var position = CurrentPosition();

      if (octetCount > RemainingBytes())
      {
        var sb = new StringBuilder("Incorrect RenderSize. ");
        sb.AppendFormat("Specified: {0}, Remaining: {1}",
          octetCount.ToString(CultureInfo.InvariantCulture),
          RemainingBytes().ToString(CultureInfo.InvariantCulture));
        throw new BerDecodeException(sb.ToString(), position);
      }

      var values = new byte[octetCount];

      try
      {
        octets.CopyTo(0, values, 0, octetCount);
        octets.RemoveRange(0, octetCount);
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }

      return values;
    }

    internal bool IsNextNull()
    {
      return 0x05 == octets[0];
    }

    internal int NextNull()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x05 != b)
        {
          var sb = new StringBuilder("Expected Null. ");
          sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        // Next octet must be 0
        b = GetNextOctet();
        if (0x00 != b)
        {
          var sb = new StringBuilder("Null has non-zero size. ");
          sb.AppendFormat("RenderSize: {0}", b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        return 0;
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal bool IsNextSequence()
    {
      return 0x30 == octets[0];
    }

    internal int NextSequence()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x30 != b)
        {
          var sb = new StringBuilder("Expected Sequence. ");
          sb.AppendFormat("Specified Identifier: {0}",
            b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var length = GetLength();
        if (length > RemainingBytes())
        {
          var sb = new StringBuilder("Incorrect Sequence RenderSize. ");
          sb.AppendFormat("Specified: {0}, Remaining: {1}",
            length.ToString(CultureInfo.InvariantCulture),
            RemainingBytes().ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        return length;
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal bool IsNextOctetString()
    {
      return 0x04 == octets[0];
    }

    internal int NextOctetString()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x04 != b)
        {
          var sb = new StringBuilder("Expected Octet String. ");
          sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var length = GetLength();
        if (length > RemainingBytes())
        {
          var sb = new StringBuilder("Incorrect Octet String RenderSize. ");
          sb.AppendFormat("Specified: {0}, Remaining: {1}",
            length.ToString(CultureInfo.InvariantCulture),
            RemainingBytes().ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        return length;
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal bool IsNextBitString()
    {
      return 0x03 == octets[0];
    }

    internal int NextBitString()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x03 != b)
        {
          var sb = new StringBuilder("Expected Bit String. ");
          sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var length = GetLength();

        // We need to consume unused bits, which is the first
        //   octet of the remaing values
        b = octets[0];
        octets.RemoveAt(0);
        length--;

        if (0x00 != b)
        { throw new BerDecodeException("The first octet of BitString must be 0", position); }

        return length;
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal bool IsNextInteger()
    {
      return 0x02 == octets[0];
    }

    internal byte[] NextInteger()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x02 != b)
        {
          var sb = new StringBuilder("Expected Integer. ");
          sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var length = GetLength();
        if (length > RemainingBytes())
        {
          var sb = new StringBuilder("Incorrect Integer RenderSize. ");
          sb.AppendFormat("Specified: {0}, Remaining: {1}",
            length.ToString(CultureInfo.InvariantCulture),
            RemainingBytes().ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        return GetOctets(length);
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }

    internal byte[] NextOID()
    {
      var position = CurrentPosition();

      try
      {
        var b = GetNextOctet();
        if (0x06 != b)
        {
          var sb = new StringBuilder("Expected Object Identifier. ");
          sb.AppendFormat("Specified Identifier: {0}",
            b.ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var length = GetLength();
        if (length > RemainingBytes())
        {
          var sb = new StringBuilder("Incorrect Object Identifier RenderSize. ");
          sb.AppendFormat("Specified: {0}, Remaining: {1}",
            length.ToString(CultureInfo.InvariantCulture),
            RemainingBytes().ToString(CultureInfo.InvariantCulture));
          throw new BerDecodeException(sb.ToString(), position);
        }

        var values = new byte[length];

        for (var i = 0; i < length; i++)
        {
          values[i] = octets[0];
          octets.RemoveAt(0);
        }

        return values;
      }

      catch (ArgumentOutOfRangeException ex)
      { throw new BerDecodeException("Error Parsing Key", position, ex); }
    }
  }
}