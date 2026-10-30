using UnityEngine;
using System.Collections;
using System;

public class CurrencyController
{
	public const string CURRENCY = "coin";
	// Số dư khởi đầu coin (T134); người cũ được EconomyService migrate ruby×10 một lần.
	public const int DEFAULT_CURRENCY = 100;
    public static Action onBalanceChanged;
    public static Action<int> onBallanceIncreased;

    public static int GetBalance()
    {
        return CPlayerPrefs.GetInt(PrefKeys.CURRENCY, DEFAULT_CURRENCY);
    }

    public static void SetBalance(int value)
    {
        CPlayerPrefs.SetInt(PrefKeys.CURRENCY, value);
        CPlayerPrefs.Save();
    }

    public static void CreditBalance(int value)
    {
        int current = GetBalance();
        SetBalance(current + value);
        if (onBalanceChanged != null ) onBalanceChanged();
        if (onBallanceIncreased != null) onBallanceIncreased(value);
    }

    public static bool DebitBalance(int value)
    {
        int current = GetBalance();
        if (current < value)
        {
            return false;
        }

        SetBalance(current - value);
        if (onBalanceChanged != null) onBalanceChanged();
        return true;
    }
}