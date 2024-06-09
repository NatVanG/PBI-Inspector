﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBIXInspectorLibrary.Part
{
    //TODO: not thread safe
    public class ContextService
    {
        private static ContextService instance = null;

        public IPBIPartQuery PartQuery { get; set; }

        public Part Part { get; set; }

        private ContextService() { }

        public static ContextService GetInstance()
        {
            if (instance == null)
            {
                instance = new ContextService();
            }
            return instance;
        }

    }
}
