using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Util
{
  public static partial class PasswordHashing
  {
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!                                                              !!
    // !!  SECURITY NOTICE! THE ORDER HERE IS IMPORTANT!               !!
    // !!                                                              !!
    // !!  These algorithms are sorted by strength. The top algorithm  !!
    // !!  is the preferred one, and the bottom one is the least       !!
    // !!  preferred. The bottom one matches all passwords, so is the  !!
    // !!  algorithm that doesn't use a prefix to identify it. It      !!
    // !!  should never be used to create new password hashes.         !!
    // !!                                                              !!
    // !!  So, make sure that newer algorithms are added to the top    !!
    // !!  and the legacy algorithm is left at the bottom!             !!
    // !!                                                              !!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private static readonly IPasswordHashingAlgorithm[] Algorithms =
    {
      new BCryptAlgorithm(),
      new LegacyAlgorithm()
    };

    public static IPasswordHashingAlgorithm PreferredAlgorithm => Algorithms[0];

    public static IPasswordHashingAlgorithm Classify(string hashedPassword)
    {
      foreach (var algorithm in Algorithms)
      {
        if (algorithm.IsMatch(hashedPassword))
          return algorithm;
      }

      throw new InvalidOperationException("Last algorithm should match all passwords");
    }

    public static string Hash(string userName, string password) => PreferredAlgorithm.Hash(userName, password);

    public static bool Verify(string userName, string password, string hashedPassword) => Classify(hashedPassword).Verify(userName, password, hashedPassword);

    public static bool ShouldRehash(string hashedPassword) => Classify(hashedPassword) != PreferredAlgorithm;
  }
}
