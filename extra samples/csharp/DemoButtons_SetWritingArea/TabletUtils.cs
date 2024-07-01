/******************************************************* 

  TabletUtils.cs
  
  This file contains utilities which are related to the
  getting or setting properties of the pad
  
  Copyright (c) 2024 Wacom Ltd. All rights reserved.
  
********************************************************/
using System;

namespace DemoButtons
{
	 class EncodingSettings
	 {
			public wgssSTU.encodingMode encodingMode;
			public bool useColor = false;

			public EncodingSettings(wgssSTU.Tablet tablet, wgssSTU.ProtocolHelper protocolHelper)
			{
				 ushort productID = tablet.getProductId();
				 wgssSTU.encodingMode encodingMode;

				 wgssSTU.encodingFlag encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(productID, 0);
				 if ((encodingFlag & (wgssSTU.encodingFlag.EncodingFlag_16bit | wgssSTU.encodingFlag.EncodingFlag_24bit)) != 0)
				 {
						if (tablet.supportsWrite())
							 useColor = true;
				 }
				 if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_24bit) != 0)
				 {
						encodingMode = tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_24bit_Bulk : wgssSTU.encodingMode.EncodingMode_24bit;
				 }
				 else if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_16bit) != 0)
				 {
						encodingMode = tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_16bit_Bulk : wgssSTU.encodingMode.EncodingMode_16bit;
				 }
				 else
				 {
						// assumes 1bit is available
						encodingMode = wgssSTU.encodingMode.EncodingMode_1bit;
				 }
			}
	 }

	 class PenDataMode
	 { 
			public static int getPenDataOptionMode(wgssSTU.Tablet tablet)
			{
				 int penDataOptionMode = 0;

				 try
				 {
						penDataOptionMode = tablet.getPenDataOptionMode();
				 }
				 catch (Exception optionModeException)
				 {
						penDataOptionMode = -1;
				 }
				 return penDataOptionMode;
			}

			public static int setPenDataOptionMode(int currentPenDataOptionMode, wgssSTU.Tablet tablet)
			{
				 int penDataOptionMode = 0;

				 // If the current option mode is TimeCount then this is a 520 so we must reset the mode
				 // to basic data only as there is no handler for TimeCount

				 switch (currentPenDataOptionMode)
				 {
						case -1:
							 // THis must be the 300 which doesn't support getPenDataOptionMode at all so only basic data
							 penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
							 break;

						case (int)PenDataOptionMode.PenDataOptionMode_None:
							 // If the current option mode is "none" then it could be any pad so try setting the full option
							 // and if it fails or ends up as TimeCount then set it to none
							 try
							 {
									tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_TimeCountSequence);
									penDataOptionMode = tablet.getPenDataOptionMode();
									if (penDataOptionMode == (int)PenDataOptionMode.PenDataOptionMode_TimeCount)
									{
										 tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
										 penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
									}
									else
									{
										 penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence;
									}
							 }
							 catch (Exception ex)
							 {
									// THis shouldn't happen but just in case...
									//m_parent.print("Using basic pen data");
									penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
							 }
							 break;

						case (int)PenDataOptionMode.PenDataOptionMode_TimeCount:
							 tablet.setPenDataOptionMode((byte)wgssSTU.penDataOptionMode.PenDataOptionMode_None);
							 penDataOptionMode = (int)PenDataOptionMode.PenDataOptionMode_None;
							 break;

						case (int)PenDataOptionMode.PenDataOptionMode_TimeCountSequence:
							 // If the current mode is timecountsequence then leave it at that
							 penDataOptionMode = currentPenDataOptionMode;
							 break;
				 }
				 return penDataOptionMode;
			}
	 }
}