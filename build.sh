#!/bin/bash

# Build script for Claims Management Platform

# Backend build
cd VerusClaims.API || exit
dotnet build
echo "Claims Management Platform - Build Script"

