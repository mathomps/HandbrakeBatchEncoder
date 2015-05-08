properties {
	$config = "Release"
	$base_dir = resolve-path "..\"
	$solution_file = "$base_dir\HandbrakeBatchEncoder.sln"
	#$buildartifacts_dir = "$base_dir\InfraCana2.Bootstrap\bin\x86\$config\"
	$platform = "Any CPU"
	
}

Framework "4.0x86"

task default -depends Build

task Clean {
	exec {
		msbuild $solution_file /t:Clean /p:Configuration=$config /p:Platform=$platform
    }
}

task Build {
    write-host "Building for config = $config"

    exec {
	    msbuild $solution_file /P:Configuration=$config /p:Platform=$platform
    }
}

task Rebuild -depends Clean,Build -description "Performs a Clean then a Build"